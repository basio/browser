using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlparser
{
    public class Errors
    {
        public int count = 0;                                    // number of errors detected
        public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
        public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

        public virtual void SynErr(int line, int col, int n)
        {
            string s;
            switch (n)
            {
                case 0: s = "EOF expected"; break;
                case 1: s = "c expected"; break;
                case 2: s = "const expected"; break;
                case 3: s = "num expected"; break;
                case 4: s = "rel expected"; break;
                case 5: s = "\";\" expected"; break;
                case 6: s = "\"create\" expected"; break;
                case 7: s = "\"table\" expected"; break;
                case 8: s = "\"(\" expected"; break;
                case 9: s = "\",\" expected"; break;
                case 10: s = "\")\" expected"; break;
                case 11: s = "\"null\" expected"; break;
                case 12: s = "\"not\" expected"; break;
                case 13: s = "\"default\" expected"; break;
                case 14: s = "\"index\" expected"; break;
                case 15: s = "\"int\" expected"; break;
                case 16: s = "\"float\" expected"; break;
                case 17: s = "\"long\" expected"; break;
                case 18: s = "\"date\" expected"; break;
                case 19: s = "\"time\" expected"; break;
                case 20: s = "\"numeric\" expected"; break;
                case 21: s = "\"char(\" expected"; break;
                case 22: s = "\"varchar(\" expected"; break;
                case 23: s = "??? expected"; break;
                case 24: s = "invalid Name_type_pair"; break;
                case 25: s = "invalid type"; break;

                default: s = "error " + n; break;
            }
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(string s)
        {
            errorStream.WriteLine(s);
            count++;
        }

        public virtual void Warning(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
        }

        public virtual void Warning(string s)
        {
            errorStream.WriteLine(s);
        }
    } // Errors



    public class FatalError : Exception
    {
        public FatalError(string m) : base(m) { }
    }
}
