using System.Collections.Generic;
using System.Linq;

namespace MPI_2.Queries
{
    internal class Queries
    {
        //static Queries()
        //{
        //    Inserts = new List<string>();
        //    for (var i = 0; i < 1000; i++)
        //        Inserts.Add("('Продукт VAIIV', 3, 2, 23, 0)");
        //}

        public static List<string> Inserts = Enumerable.Repeat("('Продукт VAIIV', 3, 2, 23, 0)", 4000).ToList();

        public static List<string> Unions = Enumerable.Repeat("Select * from Production.Products as p \r\njoin Production.Categories as c on c.categoryid = p.categoryid \r\njoin Production.Suppliers as s on s.supplierid = p.supplierid\r\nwhere p.categoryid in (1,2,3) and contactname like 'P%' ", 0)
            .Concat(Enumerable.Repeat("Select * from Production.Products as p \r\njoin Production.Categories as c on c.categoryid = p.categoryid \r\njoin Production.Suppliers as s on s.supplierid = p.supplierid\r\nwhere p.categoryid in (4,5,6) and contactname like 'S%'", 800))
            .ToList();

    }
}
