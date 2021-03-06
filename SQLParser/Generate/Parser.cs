
using System;
using System.Collections.Generic;
using System.Collections;
namespace sqlparser {



public class Parser {
	public const int _EOF = 0;
	public const int _c = 1;
	public const int _const = 2;
	public const int _num = 3;
	public const int _rel = 4;
	public const int maxT = 24;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

int flagg;
(. bool IsTable(){
														Token x=scanner.peek();
														x=scanner.peek();
													if(x.val=="table") return true;
														return false;
													}.)


	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Sql() {
		flagg=0;
		while (la.kind == 5) {
			Define_data();
		}
	}

	void Define_data() {
		if (IsTable()) {
			List<column_keyvalue> a=new List<column_keyvalue>();
			string table=""; 
			Create_table_command(a,out table);
			GO();
			if(errors.count==0){
			SQLFunc._create(table, a);
			}
		} else if (la.kind == 5) {
			Get();
			while (StartOf(1)) {
				Get();
			}
			GO();
		} else SynErr(25);
	}

	void Create_table_command(List<column_keyvalue> a,out  string table ) {
		Expect(5);
		Expect(8);
		str(out table);
		Expect(9);
		Name_type_pair(a);
		while (la.kind == 10) {
			Get();
			Name_type_pair(a);
		}
		Expect(11);
	}

	void GO() {
		while (la.kind == 6 || la.kind == 7) {
			if (la.kind == 6) {
				Get();
			} else {
				Get();
			}
		}
	}

	void str(out string  xz) {
		Expect(1);
		xz=t.val;	
	}

	void Name_type_pair(List<column_keyvalue> a) {
		string name,typ,temp,temp2;column_keyvalue kv=new column_keyvalue();
		name=typ=temp=temp2="";
		kv.isdefault=false;
		kv.isindex=false;
		kv.isnull=true;
		str(out name);
		type(out typ);
		if (la.kind == 12 || la.kind == 13) {
			if (la.kind == 12) {
				Get();
				kv.isnull=true;	  
			} else {
				Get();
				Expect(12);
				kv.isnull=false;	  
			}
		}
		if (la.kind == 14) {
			Get();
			if (la.kind == 1) {
				str(out temp2);
			} else if (la.kind == 3) {
				number(out temp2);
			} else SynErr(26);
			kv.isdefault=true;
			kv.def=temp2;        
		}
		if (la.kind == 15) {
			Get();
			kv.isindex=true;      
		}
		kv.name=name;
		kv.type=typ;
		a.Add(kv);
	}

	void type(out string  type) {
		string num;type="";
		switch (la.kind) {
		case 16: {
			Get();
			type=type+("int");
			break;
		}
		case 17: {
			Get();
			type=type+("float");
			break;
		}
		case 18: {
			Get();
			type=type+("long");
			break;
		}
		case 19: {
			Get();
			type=type+("date");
			break;
		}
		case 20: {
			Get();
			type=type+("time");
			break;
		}
		case 21: {
			Get();
			type=type+("num");
			break;
		}
		case 22: {
			Get();
			number(out num);
			type=type+("char/");
			type=type+(num);
			Expect(11);
			break;
		}
		case 23: {
			Get();
			number(out num);
			type=type+("varchar/");
			type=type+(num);
			Expect(11);
			break;
		}
		default: SynErr(27); break;
		}
	}

	void number(out string a) {
		Expect(3);
		a=t.val;
		
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Sql();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x}

	};
} // end Parser

public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "c expected"; break;
			case 2: s = "const expected"; break;
			case 3: s = "num expected"; break;
			case 4: s = "rel expected"; break;
			case 5: s = "\"create\" expected"; break;
			case 6: s = "\";\" expected"; break;
			case 7: s = "\"go\" expected"; break;
			case 8: s = "\"table\" expected"; break;
			case 9: s = "\"(\" expected"; break;
			case 10: s = "\",\" expected"; break;
			case 11: s = "\")\" expected"; break;
			case 12: s = "\"null\" expected"; break;
			case 13: s = "\"not\" expected"; break;
			case 14: s = "\"default\" expected"; break;
			case 15: s = "\"index\" expected"; break;
			case 16: s = "\"int\" expected"; break;
			case 17: s = "\"float\" expected"; break;
			case 18: s = "\"long\" expected"; break;
			case 19: s = "\"date\" expected"; break;
			case 20: s = "\"time\" expected"; break;
			case 21: s = "\"numeric\" expected"; break;
			case 22: s = "\"char(\" expected"; break;
			case 23: s = "\"varchar(\" expected"; break;
			case 24: s = "??? expected"; break;
			case 25: s = "invalid Define_data"; break;
			case 26: s = "invalid Name_type_pair"; break;
			case 27: s = "invalid type"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}


}