using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlparser
{
    class column_keyvalue
    {
        public string name;
        public string type;
        public bool isnull;
        public bool isindex;
        public bool isdefault;
        public string def;
    }
    class SQLFunc
    {

        static public void _create(string table, List<column_keyvalue> a)
        {
        }
        static public void _select(string table, List<string> columnlist, List<int> condition, List<string> l, List<string> op, List<string> r)
        {

        }
        static public void _drop(List<string> tablelist)
        {
        }        
        static public void _update(string table, Dictionary<string, string> col_table, List<int> condition, List<string> l, List<string> op, List<string> r)
        {
        }
        static public void _insert(string table, List<string> columns, List<string> values)
        {
        }
        static public void _delete(string table, List<int> condition, List<string> l, List<string> op, List<string> r)
        {
        }

    }
    /*



*/

}

