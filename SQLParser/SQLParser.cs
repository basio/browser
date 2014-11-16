
using System;
using System.Collections.Generic;
using System.Collections;
namespace sqlparser{

public class SQLParser
{
    public const int _EOF = 0;
    public const int _c = 1;
    public const int _const = 2;
    public const int _num = 3;
    public const int _rel = 4;
    public const int maxT = 23;

    const bool T = true;
    const bool x = false;
    const int minErrDist = 2;

    public SQLScanner scanner;
    public Errors errors;

    public Token t;    // last recognized token
    public Token la;   // lookahead token
    int errDist = minErrDist;

    int flagg;


    public SQLParser(SQLScanner scanner)
    {
        this.scanner = scanner;
        errors = new Errors();
    }

    void SynErr(int n)
    {
        if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
        errDist = 0;
    }

    public void SemErr(string msg)
    {
        if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
        errDist = 0;
    }

    void Get()
    {
        for (; ; )
        {
            t = la;
            la = scanner.Scan();
            if (la.kind <= maxT) { ++errDist; break; }

            la = t;
        }
    }

    void Expect(int n)
    {
        if (la.kind == n) Get(); else { SynErr(n); }
    }

    bool StartOf(int s)
    {
        return set[s, la.kind];
    }

    void ExpectWeak(int n, int follow)
    {
        if (la.kind == n) Get();
        else
        {
            SynErr(n);
            while (!StartOf(follow)) Get();
        }
    }


    bool WeakSeparator(int n, int syFol, int repFol)
    {
        int kind = la.kind;
        if (kind == n) { Get(); return true; }
        else if (StartOf(repFol)) { return false; }
        else
        {
            SynErr(n);
            while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind]))
            {
                Get();
                kind = la.kind;
            }
            return StartOf(syFol);
        }
    }


    void Sql()
    {
        flagg = 0;
        while (la.kind == 6)
        {
            Define_data();
        }
    }

    void Define_data()
    {
        List<column_keyvalue> a = new List<column_keyvalue>();
        string table = "";
        Create_command(a, out table);
        Expect(5);
        if (errors.count == 0)
        {
            //_create(table, a); flagg = 1;
        }
    }

    void Create_command(List<column_keyvalue> a, out string table)
    {
        Expect(6);
        Expect(7);
        str(out  table);
        Expect(8);
        Name_type_pair(a);
        while (la.kind == 9)
        {
            Get();
            Name_type_pair(a);
        }
        Expect(10);
    }

    void str( out string xz)
    {
        Expect(1);
        xz = t.val;
    }

    void Name_type_pair(List<column_keyvalue> a)
    {
        string name, typ, temp, temp2;
        name = typ = temp2 = temp = "";
        column_keyvalue kv = new column_keyvalue(); ;
        kv.isdefault = false;
        kv.isindex = false;
        kv.isnull = true;
        str(out  name);
        type(out typ);
        if (la.kind == 11 || la.kind == 12)
        {
            if (la.kind == 11)
            {
                Get();
                kv.isnull = true;
            }
            else
            {
                Get();
                Expect(11);
                kv.isnull = false;
            }
        }
        if (la.kind == 13)
        {
            Get();
            if (la.kind == 1)
            {
                str(out temp2);
            }
            else if (la.kind == 3)
            {
                number(out temp2);
            }
            else SynErr(24);
            kv.isdefault = true;
            kv.def = temp2;
        }
        if (la.kind == 14)
        {
            Get();
            kv.isindex = true;
        }
        kv.name = name;
        kv.type = typ;
        a.Add(kv);
    }

    void type(out string type)
    {
        string num;
        type = "";
        switch (la.kind)
        {
            case 15:
                {
                    Get();
                    type = type + ("int");
                    break;
                }
            case 16:
                {
                    Get();
                    type = type + ("float");
                    break;
                }
            case 17:
                {
                    Get();
                    type = type + ("long");
                    break;
                }
            case 18:
                {
                    Get();
                    type = type + ("date");
                    break;
                }
            case 19:
                {
                    Get();
                    type = type + ("time");
                    break;
                }
            case 20:
                {
                    Get();
                    type = type + ("num");
                    break;
                }
            case 21:
                {
                    Get();
                    number(out num);
                    type = type + ("char/");
                    type = type + (num);
                    Expect(10);
                    break;
                }
            case 22:
                {
                    Get();
                    number(out num);
                    type = type + ("varchar/");
                    type = type + (num);
                    Expect(10);
                    break;
                }
            default: SynErr(25); break;
        }
    }

    void number(out string a)
    {
        Expect(3);
        a = t.val;

    }



    public void Parse()
    {
        la = new Token();
        la.val = "";
        Get();
        Sql();
        Expect(0);

    }

    static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}

	};
} // end Parser


}