using System;

namespace ConsoleTable
{
    public class RowWithoutColumn
    {
        public RowWithoutColumn(string value, RowOrder rowOrder = RowOrder.Before)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value), "The value must be non-null and non-empty");
            Value = value;
            RowOrder = rowOrder;
        }

        public RowOrder RowOrder { get; set; }
        public string Value { get; }
    }
}