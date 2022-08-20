using System;
using System.Collections.Generic;

namespace ConsoleTable
{
    public class ConsoleTableOptions
    {
        public readonly Dictionary<object, string> CustomFormats;
        public bool SeparateEachRow;
        public char Separator;

        public ConsoleTableOptions()
        {
            CustomFormats = new Dictionary<object, string>();
            RowsWithoutColumns = new Dictionary<int, List<string>>();
            Alignments = Array.Empty<Alignment>();
            Separator = '-';
        }

        public Alignment[] Alignments { get; set; }
        public Dictionary<int, List<string>> RowsWithoutColumns { get; }
    }
}