using System;
using System.Collections.Generic;
using System.Globalization;

namespace ConsoleTable
{
    public class ConsoleTableOptions
    {
        public ConsoleTableOptions() : this(CultureInfo.CurrentCulture)
        {
        }

        public ConsoleTableOptions(CultureInfo cultureInfo)
        {
            CustomFormats = new Dictionary<object, string>();
            RowsWithoutColumns = new Dictionary<int, List<RowWithoutColumn>>();
            Alignments = Array.Empty<Alignment>();
            Separator = '-';
            CultureInfo = cultureInfo;
            NestedTables = new Dictionary<int, List<Table>>();
            EnableAutoIndentForNestedTables = true;
        }

        public Dictionary<object, string> CustomFormats { get; }
        public bool SeparateEachRow { get; set; }
        public char Separator { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public Alignment[] Alignments { get; set; }
        public Dictionary<int, List<RowWithoutColumn>> RowsWithoutColumns { get; }
        public Dictionary<int, List<Table>> NestedTables { get; }
        public bool EnableAutoIndentForNestedTables { get; set; }
    }
}