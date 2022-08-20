# ConsoleTable
A library for simple table formatting
## Install
### PM
```
Install-Package ConsoleTable
```
### .NET CLI
```
dotnet add package ConsoleTable --version 1.0.0
```
## Example usage
```
            var table = new Table()
                .AddColumn("#", "Name", "Surname", "Growth")
                .AddRow(1, "Name1", "Surname1", "120")
                .AddRow(2, "Name2", "Surname2", "10")
                .AddRow(3, "Name333333", "Surname3333", "0");
            Console.WriteLine(table);
```
## Console Output
![image](https://user-images.githubusercontent.com/84572791/185746912-9ce2d5c6-0706-48ea-94a7-9022c7577dc3.png)

## Example usage
Aligning each columns on different positions(Left,Right,Center)
```
            var table = new Table()
                .AddColumn("#", "Name", "Surname", "Growth")
                .AddAlignment(Alignment.Center,2)
                .AddAlignment(Alignment.Right,3)
                .AddRow(1, "Name1", "Surname1", "120")
                .AddRow(2, "Name2", "Surname2", "10")
                .AddRow(3, "Name333333", "Surname33333333asdasd", "0");
            Console.WriteLine(table);
 ```
 ## Console Output
![image](https://user-images.githubusercontent.com/84572791/185746902-bbbe7d35-0156-45ff-b89d-11fce8bf5a53.png)
 ## Example usage
 Using custom formatting for each column
 ```
            var table = new Table()
                .AddColumn("#", "Name", "Surname", "Growth", "Birth date")
                .AddCustomFormat(typeof(decimal), "{0:0.00}")
                .AddCustomFormat(4,"{0:yyyy.MM.dd}")
                .AddRow(1, "Name1", "Surname1", 120.95M,DateTime.Parse("02/02/2002 00:00:00"))
                .AddRow(2, "Name2", "Surname2", 10.31123M,DateTime.Parse("05/02/2003"))
                .AddRow(3, "Name333333", "Surname33333333asdasd",13.2M,DateTime.Parse("05/02/2012"));
            Console.WriteLine(table);
  ```
  ## Console Output
  ![image](https://user-images.githubusercontent.com/84572791/185746879-8730c647-bb74-453e-a058-1a1ddd20a07e.png)
  ## Example usage
  Using nested tables and rows
  ```
            var table = new Table()
                .AddColumn("#", "Name","Surname")
                .AddRow(1, "First name","Surname one")
                .AddRowWithoutColumn("Products: ")
                .AddRowWithoutColumn("Anything text", RowOrder.After);
            var table2 = new Table()
                .AddColumn("#", "Product name")
                .AddRow(1, "Beer")
                .AddRow(2, "M16A4")
                .SetStandardPadding(10);
            table.AddTable(table2)
                .AddRow(2,"Second name","Surname two")
                .AddTable(table2)
                .AddRowWithoutColumn("Products: ")
                .AddRowWithoutColumn("Anything text", RowOrder.After)
                .AddRow(3,"3d name","Surname three")
                .AddRowWithoutColumn("Anything text", RowOrder.After)
                .AddTable(table2)
                .AddRowWithoutColumn("Products: ")
                .SetAutoInsteadForNestedTables(false)
                .AddAlignment(Alignment.Center,1)
                .AddSeparatorForEachRow();
            Console.WriteLine(table.ToString());
  ```
  ## Console Output
  ![image](https://user-images.githubusercontent.com/84572791/185747348-86086287-9cd3-4541-a269-8cd783e86434.png)

See more in [ExampleProject](https://github.com/sakozoko/ConsoleTable/tree/master/ExampleProject)
