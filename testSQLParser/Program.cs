using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sqlparser;
namespace testSQLParser
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLScanner scanner = new SQLScanner("d.sql");
            SQLParser parser = new SQLParser(scanner);
            //parser.tab = new SymbolTable(parser);
            //parser.gen = new CodeGenerator();
            parser.Parse();
            if (parser.errors.count == 0)
            {
                //parser.gen.Decode();
                //parser.gen.Interpret("Taste.IN");

            }
            else
            {
            //    foreach (var e in parser.errors)
              //  {
                //    Console.WriteLine(e);
                //}
            }
        }
    }
}
