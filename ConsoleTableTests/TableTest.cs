using System.Security.Cryptography.X509Certificates;
using ConsoleTable;
using Xunit;

namespace ConsoleTableTests;

public class TableTest
{
    [Fact]
    public void AlignmentTest()
    {
        var table = new Table()
            .AddColumn("#", "Name", "Surname", "Growth")
            .AddAlignment(Alignment.Left)
            .AddAlignment(Alignment.Right, 1)
            .AddAlignment(Alignment.Center, 2)
            .AddRow(1, "Name1", "Surname1", "120")
            .AddRow(2, "Name2", "Surname2", "10")
            .AddRow(3, "Name333333", "Surname3333", "0");

        var actual = table.ToString();
        const string expected = "| # |       Name |   Surname   | Growth |\r\n" +
                                "-----------------------------------------\n" +
                                "| 1 |      Name1 |  Surname1   | 120    |\n" +
                                "| 2 |      Name2 |  Surname2   | 10     |\n" +
                                "| 3 | Name333333 | Surname3333 | 0      |";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CustomFormattingTest()
    {
        var table = new Table()
            .AddColumn("#", "Name", "Surname", "Growth", "Date")
            .AddCustomFormat(typeof(int), "{0:0.01}")
            .AddCustomFormat(1, " # {0} # ")
            .AddCustomFormat(3, "{0:0.00}")
            .AddCustomFormat(typeof(DateTime), "{0:HH:mm}")
            .AddRow(1, "Name1", "Surname", 3.22, DateTime.Parse("2002/02/04 15:32"))
            .AddRow(2, "Name2", "Surname22", 5.9, DateTime.Parse("2002.02.03 12:32"));

        var actual = table.ToString();
        const string expected = "| #    | Name        | Surname   | Growth | Date  |\r\n" +
                                "---------------------------------------------------\n" +
                                "| 1,01 |  # Name1 #  | Surname   | 3,22   | 15:32 |\n" +
                                "| 2,01 |  # Name2 #  | Surname22 | 5,90   | 12:32 |";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddingRowsWithoutColumnsTest()
    {
        var table = new Table()
            .AddColumn("#", "Name")
            .AddRow("1", "Namee")
            .AddRowWithoutColumn("First row without columns")
            .AddRowWithoutColumn("Second row without columns")
            .AddRow("2", "Namee2");

        var actual = table.ToString();
        const string expected = "| # | Name   |\r\n" +
                                "--------------\n" +
                                "| 1 | Namee  |\n" +
                                "First row without columns\n" +
                                "Second row without columns\n" +
                                "| 2 | Namee2 |";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddingTableToTableTest()
    {
        var table = new Table()
            .AddColumn("#", "Name")
            .AddRow(1, "First name");
        var table2 = new Table()
            .AddColumn("##", "Surnames")
            .AddRow("1", "Surnamee");
        table.AddTable(table2)
            .AddRow(2, "Second namee")
            .AddTable(table2);

        var actual = table.ToString();
        const string expected = "| # | Name         |\r\n" +
                                "--------------------\n" +
                                "| 1 | First name   |\n" +
                                "      | ## | Surnames |\r\n" +
                                "      -----------------\n" +
                                "      | 1  | Surnamee |\n" +
                                "| 2 | Second namee |\n" +
                                "      | ## | Surnames |\r\n" +
                                "      -----------------\n" +
                                "      | 1  | Surnamee |";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddingTableToTableWithCustomPaddingTest()
    {
        var table = new Table()
            .AddColumn("#", "Name")
            .AddRow(1, "First name");
        var table2 = new Table()
            .AddColumn("##", "Surnames")
            .AddRow("1", "Surnamee");
        table.AddTable(table2)
            .AddRow(2, "Second namee")
            .AddTable(table2.SetStandardPadding(10))
            .SetAutoInsteadForNestedTables(false)
            .SetStandardPadding(5);

        var actual = table.ToString();
        const string expected = "     | # | Name         |\r\n" +
                                "     --------------------\n" +
                                "     | 1 | First name   |\n" +
                                "          | ## | Surnames |\r\n" +
                                "          -----------------\n" +
                                "          | 1  | Surnamee |\n" +
                                "     | 2 | Second namee |\n" +
                                "          | ## | Surnames |\r\n" +
                                "          -----------------\n" +
                                "          | 1  | Surnamee |";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddingTableToTableWithNestedRowsTest()
    {
        var table = new Table()
            .AddColumn("#", "Name")
            .AddRow(1, "First name");
        var table2 = new Table()
            .AddColumn("##", "Surnames")
            .AddRow("1", "Surnamee");
        var addingRowBeforeTable = new RowWithoutColumn("something value");
        var addingRowAfterTable = new RowWithoutColumn("something value2", RowOrder.After);
        table.AddTable(table2)
            .AddRowWithoutColumn(addingRowAfterTable)
            .AddRowWithoutColumn(addingRowBeforeTable)
            .AddRow(2, "Second namee")
            .AddRowWithoutColumn(addingRowBeforeTable)
            .AddRowWithoutColumn(addingRowAfterTable)
            .AddTable(table2);

        var actual = table.ToString();
        const string expected = "| # | Name         |\r\n" +
                                "--------------------\n" +
                                "| 1 | First name   |\n" +
                                "something value\n" +
                                "      | ## | Surnames |\r\n" +
                                "      -----------------\n" +
                                "      | 1  | Surnamee |\n" +
                                "something value2\n" +
                                "| 2 | Second namee |\n" +
                                "something value\n" +
                                "      | ## | Surnames |\r\n" +
                                "      -----------------\n" +
                                "      | 1  | Surnamee |\n" +
                                "something value2";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SeparateEachRowTest()
    {
        var table = new Table()
            .AddColumn("#", "Name")
            .AddRow(1, "name")
            .AddRow(2, "Sooo long row")
            .AddSeparatorForEachRow();

        var actual = table.ToString();
        const string expected = "| # | Name          |\r\n" +
                                "---------------------\n" +
                                "| 1 | name          |\n" +
                                "---------------------\n" +
                                "| 2 | Sooo long row |\n" +
                                "---------------------";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SeparateEachRowWithCustomSeparatingTest()
    {
        var table = new Table()
            .AddColumn("#", "Name")
            .AddRow(1, "name")
            .AddRow(2, "Sooo long row")
            .AddSeparatorForEachRow('.');

        var actual = table.ToString();
        const string expected = "| # | Name          |\r\n" +
                                ".....................\n" +
                                "| 1 | name          |\n" +
                                ".....................\n" +
                                "| 2 | Sooo long row |\n" +
                                ".....................";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CreatingTableFromEnumerableDto()
    {
        var collection = Enumerable.Repeat(new TestDto
        {
            Name = "Name",
            Salary = 35,
            Surname = "Surname"

        }, 3);
        var actual = Table.From(collection)
            .SetStandardPadding(1)
            .ToString();
        const string expected = " | Name | Surname | Salary |\r\n" +
                                " ---------------------------\n" +
                                " | Name | Surname | 35     |\n" +
                                " | Name | Surname | 35     |\n" +
                                " | Name | Surname | 35     |";
        Assert.Equal(expected,actual);

    }

    public class TestDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public decimal Salary { get; set; }
    }
}