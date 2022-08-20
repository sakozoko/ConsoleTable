using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ConsoleTable
{
    public class Table
    {
        private readonly ConsoleTableOptions _consoleTableOptions;
        private List<int> _currentPaddings;

        public Table() : this(new ConsoleTableOptions())
        {
        }

        public Table(ConsoleTableOptions consoleTableOptions)
        {
            Columns = new List<string>();
            Rows = new List<object[]>();
            _consoleTableOptions = consoleTableOptions;
        }

        public List<int> CurrentPaddings
        {
            get
            {
                if (_currentPaddings is null)
                    SetCurrentPadding();
                return _currentPaddings;
            }
        }

        private int StandardPadding { get; set; }
        private List<string> Columns { get; }
        private List<object[]> Rows { get; }
        private List<object[]> ModifiedRows { get; set; }

        public Table AddColumn(params string[] strings)
        {
            foreach (var s in strings.Where(x => x != null)) Columns.Add(s);

            if (_consoleTableOptions.Alignments.Length == 0)
            {
                _consoleTableOptions.Alignments = new Alignment[Columns.Count];
            }
            else
            {
                var t = new Alignment[Columns.Count];
                Array.Copy(_consoleTableOptions.Alignments, t, _consoleTableOptions.Alignments.Length);
                _consoleTableOptions.Alignments = t;
            }

            return this;
        }

        public Table AddCustomFormat(Type targetType, string format)
        {
            if (targetType is null || string.IsNullOrWhiteSpace(format))
                throw new ArgumentNullException(targetType is null ? nameof(targetType) : nameof(format),
                    "Value is null");
            if (_consoleTableOptions.CustomFormats.ContainsKey(targetType))
                _consoleTableOptions.CustomFormats[targetType] = format;
            else
                _consoleTableOptions.CustomFormats.Add(targetType, format);
            return this;
        }

        public Table AddCustomFormat(int indexOfTableColumn, string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentNullException(nameof(format), nameof(format) + " is null");

            if (indexOfTableColumn < 0)
                throw new ArgumentException("Must be greater than or equal to 0", nameof(indexOfTableColumn));
            if (_consoleTableOptions.CustomFormats.ContainsKey(indexOfTableColumn))
                _consoleTableOptions.CustomFormats[indexOfTableColumn] = format;
            else
                _consoleTableOptions.CustomFormats.Add(indexOfTableColumn, format);
            return this;
        }

        public Table AddAlignment(Alignment alignment)
        {
            for (var i = 0; i < _consoleTableOptions.Alignments.Length; i++)
                _consoleTableOptions.Alignments[i] = alignment;
            return this;
        }

        public Table AddAlignment(Alignment alignment, int indexOfColumn)
        {
            if (_consoleTableOptions.Alignments.Length > indexOfColumn)
            {
                _consoleTableOptions.Alignments[indexOfColumn] = alignment;
                return this;
            }

            throw new IndexOutOfRangeException("Index of column greater than column count");
        }

        public Table AddRow(params object[] strings)
        {
            Rows.Add(strings);

            return this;
        }

        public Table AddSeparatorForEachRow()
        {
            _consoleTableOptions.SeparateEachRow = true;

            return this;
        }

        public Table AddSeparatorForEachRow(char separator)
        {
            _consoleTableOptions.SeparateEachRow = true;
            _consoleTableOptions.Separator = separator;
            return this;
        }

        public Table AddCultureInfo(CultureInfo cultureInfo)
        {
            _consoleTableOptions.CultureInfo = cultureInfo ?? CultureInfo.CurrentCulture;
            return this;
        }

        public Table AddRowWithoutColumn(string str, RowOrder rowOrder = RowOrder.Before)
        {
            return AddRowWithoutColumn(new RowWithoutColumn(str, rowOrder));
        }

        public Table AddRowWithoutColumn(RowWithoutColumn row)
        {
            var currentLastIndex = Rows.Count - 1;
            if (_consoleTableOptions.RowsWithoutColumns.ContainsKey(currentLastIndex))
            {
                _consoleTableOptions.RowsWithoutColumns[currentLastIndex].Add(row);
                return this;
            }

            var list = new List<RowWithoutColumn> { row };
            _consoleTableOptions.RowsWithoutColumns.Add(currentLastIndex, list);
            return this;
        }

        public Table SetAutoInsteadForNestedTables(bool flag)
        {
            _consoleTableOptions.EnableAutoIndentForNestedTables = flag;
            return this;
        }

        public Table AddTable(Table table)
        {
            var currentLastIndex = Rows.Count - 1;
            if (_consoleTableOptions.NestedTables.ContainsKey(currentLastIndex))
            {
                _consoleTableOptions.NestedTables[currentLastIndex].Add(table);
                return this;
            }

            var list = new List<Table> { table };
            _consoleTableOptions.NestedTables.Add(currentLastIndex, list);
            return this;
        }

        public Table SetStandardPadding(int value)
        {
            if (value < 0)
                throw new ArgumentException("The value must be greater than 0", nameof(value));
            StandardPadding = value;
            return this;
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            CreateModifyingRowsForCustomFormats();
            SetCurrentPadding();

            var format = GetFormat();

            var header = GetHeader(format);

            var formattedRows = GetFormattedRows(format);

            var longestLine = Math.Max(header.Length, Rows.Any() ? formattedRows.Max(row => row.Length) : 0);
            var divider = new string(' ', StandardPadding) + string.Join("",
                new string(_consoleTableOptions.Separator, longestLine));

            header = new string(' ', StandardPadding) + header;
            formattedRows = formattedRows.Select(x => new string(' ', StandardPadding) + x).ToList();

            strBuilder.AppendLine(header);
            strBuilder.Append(divider);
            for (var i = 0; i < formattedRows.Count; i++)
            {
                strBuilder.Append("\n" + formattedRows[i]);

                AddNestedElements(strBuilder, i);

                if (_consoleTableOptions.SeparateEachRow)
                    strBuilder.Append("\n" + divider);
            }

            return strBuilder.ToString();
        }

        private void AddNestedElements(StringBuilder stringBuilder, int indexOfRow)
        {
            if (_consoleTableOptions.RowsWithoutColumns.ContainsKey(indexOfRow))
                foreach (var str in _consoleTableOptions.RowsWithoutColumns[indexOfRow]
                             .Where(x => x.RowOrder == RowOrder.Before))
                    stringBuilder.Append("\n" + str.Value);

            AddNestedTables(stringBuilder, indexOfRow);

            if (_consoleTableOptions.RowsWithoutColumns.ContainsKey(indexOfRow))
                foreach (var str in _consoleTableOptions.RowsWithoutColumns[indexOfRow]
                             .Where(x => x.RowOrder == RowOrder.After))
                    stringBuilder.Append("\n" + str.Value);
        }

        private void AddNestedTables(StringBuilder stringBuilder, int indexOfRow)
        {
            if (!_consoleTableOptions.NestedTables.ContainsKey(indexOfRow)) return;
            foreach (var table in _consoleTableOptions.NestedTables[indexOfRow])
            {
                if (_consoleTableOptions.EnableAutoIndentForNestedTables)
                    table.SetStandardPadding(_currentPaddings?.Sum() / 2 ?? 0);

                stringBuilder.Append("\n" + table);
            }
        }

        private string RightOrLeftAlign(int indexOfColumn)
        {
            return _consoleTableOptions.Alignments[indexOfColumn] == Alignment.Left ? "-" : "";
        }

        private string GetFormat()
        {
            return Columns.Select((_, i) => "| {"
                                            + i + (_consoleTableOptions.Alignments[i] != Alignment.Center
                                                ? "," + RightOrLeftAlign(i) + CurrentPaddings[i]
                                                : ""
                                            ) + "} ")
                .Aggregate((s, a) => s + a) + "|";
        }

        private string GetHeader(string format)
        {
            return string.Format(format, Columns.Select((c, i) => _consoleTableOptions.Alignments[i] == Alignment.Center
                    ? new string(' ', (CurrentPaddings[i] - c.Length) / 2) + c +
                      new string(' ', CurrentPaddings[i] - c.Length - (CurrentPaddings[i] - c.Length) / 2)
                    : c)
                .ToArray<object>());
        }

        private List<string> GetFormattedRows(string format)
        {
            return ModifiedRows.Select(row =>
                string.Format(format, row.Select((c, i) => _consoleTableOptions.Alignments[i] == Alignment.Center
                    ? new string(' ', (CurrentPaddings[i] - c.ToString().Length) / 2) + c +
                      new string(' ',
                          CurrentPaddings[i] - c.ToString().Length -
                          (CurrentPaddings[i] - c.ToString().Length) / 2)
                    : c
                ).ToArray())).ToList();
        }

        private void CreateModifyingRowsForCustomFormats()
        {
            ModifiedRows = Rows.Select(x =>
                x.Select((v, i) =>
                    _consoleTableOptions.CustomFormats.ContainsKey(i)
                        ? string.Format($"{_consoleTableOptions.CustomFormats[i]}", v, _consoleTableOptions.CultureInfo)
                        : v).Select(v =>
                    _consoleTableOptions.CustomFormats.ContainsKey(v.GetType())
                        ? string.Format($"{_consoleTableOptions.CustomFormats[v.GetType()]}", v,
                            _consoleTableOptions.CultureInfo)
                        : v).ToArray()).ToList();
        }


        private void SetCurrentPadding()
        {
            CreateModifyingRowsForCustomFormats();
            _currentPaddings = Columns
                .Select((_, i) => ModifiedRows.Select(row => row[i])
                    .Union(new[] { Columns[i] })
                    .Where(value => value != null)
                    .Select(value => value.ToString().Length).Max()).ToList();
        }
    }
}