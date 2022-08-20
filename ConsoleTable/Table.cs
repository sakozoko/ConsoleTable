using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTable
{
    public class Table
    {
        private readonly ConsoleTableOptions _consoleTableOptions;
        private List<int> _currentPadding;

        public Table() : this(new ConsoleTableOptions())
        {
        }

        public Table(ConsoleTableOptions consoleTableOptions) : this(new List<int>(), consoleTableOptions)
        {
        }

        public Table(IEnumerable<int> paddings) : this(paddings, new ConsoleTableOptions())
        {
        }

        public Table(IEnumerable<int> paddings, ConsoleTableOptions consoleTableOptions)
        {
            Columns = new List<string>();
            Rows = new List<object[]>();
            Paddings = paddings.ToList();
            _consoleTableOptions = consoleTableOptions;
        }

        public List<int> CurrentPadding
        {
            get
            {
                if (_currentPadding is null)
                    SetCurrentPadding();
                return _currentPadding;
            }
        }

        private List<int> Paddings { get; }
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

        public Table AddRowWithoutColumn(string str)
        {
            var currentLastIndex = Rows.Count - 1;
            if (_consoleTableOptions.RowsWithoutColumns.ContainsKey(currentLastIndex))
            {
                _consoleTableOptions.RowsWithoutColumns[currentLastIndex].Add(str);
                return this;
            }

            var list = new List<string> { str };
            _consoleTableOptions.RowsWithoutColumns.Add(currentLastIndex, list);
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
            var divider = new string(' ', Paddings?.Sum() / 2 ?? 0) + " " +
                          string.Join("", Enumerable.Repeat(_consoleTableOptions.Separator, longestLine - 1)) + " ";
            //add indent if Padding property not null
            header = new string(' ', Paddings?.Sum() / 2 ?? 0) + header;
            formattedRows = formattedRows.Select(x => new string(' ', Paddings?.Sum() / 2 ?? 0) + x).ToList();

            //creating resulted string
            strBuilder.AppendLine(header);
            strBuilder.Append(divider);
            for (var i = 0; i < formattedRows.Count; i++)
            {
                strBuilder.Append("\n" + formattedRows[i]);
                if (_consoleTableOptions.RowsWithoutColumns.ContainsKey(i))
                    foreach (var str in _consoleTableOptions.RowsWithoutColumns[i])
                        strBuilder.Append("\n" + str);
                if (_consoleTableOptions.SeparateEachRow)
                    strBuilder.Append("\n" + divider);
            }

            return strBuilder.ToString();
        }

        private string RightOrLeftAlign(int indexOfColumn)
        {
            return _consoleTableOptions.Alignments[indexOfColumn] == Alignment.Left ? "-" : "";
        }

        private string GetFormat()
        {
            return Enumerable.Range(0, Columns.Count)
                .Select(i => " | {"
                             + i + (_consoleTableOptions.Alignments[i] != Alignment.Center
                                 ? "," + RightOrLeftAlign(i) + CurrentPadding[i]
                                 : ""
                             ) + "}")
                .Aggregate((s, a) => s + a) + " |";
        }

        private string GetHeader(string format)
        {
            return string.Format(format, Columns.Select((c, i) => _consoleTableOptions.Alignments[i] == Alignment.Center
                    ? new string(' ', (CurrentPadding[i] - c.Length) / 2) + c +
                      new string(' ', CurrentPadding[i] - c.Length - (CurrentPadding[i] - c.Length) / 2)
                    : c)
                .ToArray<object>());
        }

        private List<string> GetFormattedRows(string format)
        {
            return ModifiedRows.Select(row =>
                string.Format(format, row.Select((c, i) => _consoleTableOptions.Alignments[i] == Alignment.Center
                    ? new string(' ', (CurrentPadding[i] - c.ToString().Length) / 2) + c +
                      new string(' ',
                          CurrentPadding[i] - c.ToString().Length -
                          (CurrentPadding[i] - c.ToString().Length) / 2)
                    : c
                ).ToArray())).ToList();
        }

        private void CreateModifyingRowsForCustomFormats()
        {
            ModifiedRows = Rows.Select(x =>
                x.Select((v, i) =>
                    _consoleTableOptions.CustomFormats.ContainsKey(i)
                        ? string.Format($"{_consoleTableOptions.CustomFormats[i]}", v)
                        : v).Select(v =>
                    _consoleTableOptions.CustomFormats.ContainsKey(v.GetType())
                        ? string.Format($"{_consoleTableOptions.CustomFormats[v.GetType()]}", v)
                        : v).ToArray()).ToList();
        }


        private void SetCurrentPadding()
        {
            CreateModifyingRowsForCustomFormats();
            _currentPadding = Columns
                .Select((_, i) => ModifiedRows.Select(row => row[i])
                    .Union(new[] { Columns[i] })
                    .Where(value => value != null)
                    .Select(value => value.ToString().Length).Max())
                .ToList();
        }
    }
}