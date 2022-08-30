using System;
using ConsoleTable;

namespace ExampleProject
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //first example
            var table = new Table()
                .AddColumn("#", "Name", "Surname", "Growth")
                .AddRow(1, "Name1", "Surname1", "120")
                .AddRow(2, "Name2", "Surname2", "10")
                .AddRow(3, "Name333333", "Surname3333", "0");
            Console.WriteLine(table);
            //second example
            var table2 = new Table()
                .AddColumn("#", "Name", "Surname", "Growth")
                .AddAlignment(Alignment.Center, 2)
                .AddAlignment(Alignment.Right, 3)
                .AddRow(1, "Name1", "Surname1", "120")
                .AddRow(2, "Name2", "Surname2", "10")
                .AddRow(3, "Name333333", "Surname33333333asdasd", "0");
            Console.WriteLine(table2);
            //third example
            var table3 = new Table()
                .AddColumn("#", "Name", "Surname", "Growth", "Birth date")
                .AddCustomFormat(typeof(decimal), "{0:0.00}")
                .AddCustomFormat(4, "{0:yyyy.MM.dd}")
                .AddRow(1, "Name1", "Surname1", 120.95M, DateTime.Parse("02/02/2002 00:00:00"))
                .AddRow(2, "Name2", "Surname2", 10.31123M, DateTime.Parse("05/02/2003"))
                .AddRow(3, "Name333333", "Surname33333333asdasd", 13.2M, DateTime.Parse("05/02/2012"));
            Console.WriteLine(table3);
            //fourth example
            var table4 = new Table()
                .AddColumn("#", "Name", "Surname")
                .AddRow(1, "First name", "Surname one")
                .AddRowWithoutColumn("Products: ")
                .AddRowWithoutColumn("Anything text", RowOrder.After);
            var table42 = new Table()
                .AddColumn("#", "Product name")
                .AddRow(1, "Beer")
                .AddRow(2, "M16A4")
                .SetStandardPadding(10);
            table4.AddTable(table42)
                .AddRow(2, "Second name", "Surname two")
                .AddTable(table42)
                .AddRowWithoutColumn("Products: ")
                .AddRowWithoutColumn("Anything text", RowOrder.After)
                .AddRow(3, "3d name", "Surname three")
                .AddRowWithoutColumn("Anything text", RowOrder.After)
                .AddTable(table42)
                .AddRowWithoutColumn("Products: ")
                .SetAutoInsteadForNestedTables(false)
                .AddAlignment(Alignment.Center, 1)
                .AddSeparatorForEachRow();
            Console.WriteLine(table4.ToString());
            //fifth example
            var table5 = new Table()
                .AddColumn("#", "Name")
                .AddRowWithoutColumn("Anything text")
                .AddRow("1", "Namee");
            Console.WriteLine(table5.ToString());
        }
    }
}