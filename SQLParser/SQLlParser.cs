#include<map>
#include<string>
#include<iostream>
#include<vector>
#include "sqlFunc.h"
using namespace std;


using System;



public class Parser {
	public const int _EOF = 0;
	public const int _c = 1;
	public const int _const = 2;
	public const int _num = 3;
	public const int _rel = 4;
	public const int maxT = 38;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

int flagg;


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
		if (StartOf(1)) {
			Modify_data();
		} else if (la.kind == 6) {
			Define_data();
		} else SynErr(39);
	}

	void Modify_data() {
		if (la.kind == 23) {
			Select_command();
		} else if (la.kind == 32) {
			Insert_command();
		} else if (la.kind == 29) {
			Update_command();
		} else if (la.kind == 35) {
			Delete_command();
		} else if (la.kind == 22) {
			Drop_command();
		} else SynErr(40);
	}

	void Define_data() {
		vector<ckval> a;
		string table; 
		Create_command(a,table);
		Expect(5);
		if(errors->count==0){
		_create(table,a);flagg=1;}
	}

	void Create_command(vector<ckval> &a, string &table ) {
		Expect(6);
		Expect(7);
		str(table);
		Expect(8);
		Name_type_pair(a);
		while (la.kind == 9) {
			Get();
			Name_type_pair(a);
		}
		Expect(10);
	}

	void Select_command() {
		vector<string> columnlist;
		vector<string> a,b,c;int count=0;
		string table,temp;vector<int> condition;
		Expect(23);
		if (la.kind == 24 || la.kind == 25) {
			if (la.kind == 24) {
				Get();
			} else {
				Get();
			}
		}
		if (la.kind == 1) {
			str_list(columnlist);
			while (la.kind == 9) {
				Get();
				str_list(columnlist);
			}
		} else if (la.kind == 26) {
			Get();
			columnlist.push_back("*");
		} else SynErr(41);
		Expect(27);
		str(table);
		if (la.kind == 28) {
			Get();
			Condition(condition,a,b,c,count);
		}
		Expect(5);
		if(errors->count==0){
		_select(table,columnlist,condition,a,b,c);flagg=1;}
	}

	void Insert_command() {
		string table;
		vector<string> columns,values;
		Expect(32);
		Expect(33);
		str(table);
		Expect(8);
		str_list(columns);
		while (la.kind == 9) {
			Get();
			str_list(columns);
		}
		Expect(10);
		Expect(34);
		Expect(8);
		if (la.kind == 2) {
			strconst_list(values);
		} else if (la.kind == 3) {
			number_list(values);
		} else SynErr(42);
		while (la.kind == 9) {
			Get();
			if (la.kind == 2) {
				strconst_list(values);
			} else if (la.kind == 3) {
				number_list(values);
			} else SynErr(43);
		}
		Expect(10);
		Expect(5);
		if(errors->count==0){
		_insert(table,columns,values);flagg=1;}
	}

	void Update_command() {
		map<string,string> col_table;
		vector<string> a,b,c;int count=0;
		string table;vector<int> condition; 
		Expect(29);
		str(table);
		Expect(30);
		key_value(col_table);
		while (la.kind == 1) {
			key_value(col_table);
		}
		if (la.kind == 28) {
			Get();
			Condition(condition,a,b,c,count);
		}
		Expect(5);
		if(errors->count==0){
		_update(table,col_table,condition,a,b,c);flagg=1;}
	}

	void Delete_command() {
		string table;vector<int> condition;int count=0;
		vector<string> a,b,c; 
		Expect(35);
		Expect(27);
		str(table);
		if (la.kind == 28) {
			Get();
			Condition(condition,a,b,c,count);
		}
		Expect(5);
		if(errors->count==0){
		_delete(table,condition,a,b,c);flagg=1;}
	}

	void Drop_command() {
		vector<string> tablelist;string table;
		Expect(22);
		str_list(tablelist);
		while (la.kind == 9) {
			Get();
			str_list(tablelist);
		}
		Expect(5);
		if(errors->count==0){
		_drop(tablelist);
		flagg=1;}
	}

	void str(string & xz) {
		Expect(1);
		xz.append(coco_string_create_char(t->val));
	}

	void Name_type_pair(vector<ckval> &a) {
		string name,typ,temp,temp2;ckval kv;
		kv.isdefault=false;
		kv.isindex=false;
		kv.isnull=true;
		str(name);
		type(typ);
		if (la.kind == 11 || la.kind == 12) {
			if (la.kind == 11) {
				Get();
				kv.isnull=true;	  
			} else {
				Get();
				Expect(11);
				kv.isnull=false;	  
			}
		}
		if (la.kind == 13) {
			Get();
			if (la.kind == 1) {
				str(temp2);
			} else if (la.kind == 3) {
				number(temp2);
			} else SynErr(44);
			kv.isdefault=true;
			kv.def=temp2;        
		}
		if (la.kind == 14) {
			Get();
			kv.isindex=true;      
		}
		kv.name=name;
		kv.type=typ;
		a.push_back(kv);
	}

	void type(string & type) {
		string num;
		switch (la.kind) {
		case 15: {
			Get();
			type.append("int");
			break;
		}
		case 16: {
			Get();
			type.append("float");
			break;
		}
		case 17: {
			Get();
			type.append("long");
			break;
		}
		case 18: {
			Get();
			type.append("date");
			break;
		}
		case 19: {
			Get();
			type.append("time");
			break;
		}
		case 20: {
			Get();
			number(num);
			type.append("char/");
			type.append(num);
			Expect(10);
			break;
		}
		case 21: {
			Get();
			number(num);
			type.append("varchar/");
			type.append(num);
			Expect(10);
			break;
		}
		default: SynErr(45); break;
		}
	}

	void number(string &a) {
		Expect(3);
		a.append(coco_string_create_char(t->val));
		
	}

	void str_list(vector<string> &list) {
		string temp;
		str(temp);
		list.push_back(temp);
	}

	void Condition(vector<int> &condition,vector<string> &a,vector<string> &b,vector<string> &c ,int &count) {
		int temp; 
		con(condition,a,b,c,count);
		while (la.kind == 36 || la.kind == 37) {
			if (la.kind == 36) {
				temp=-1;
				Get();
			} else {
				temp=-2;
				Get();
			}
			con(condition,a,b,c,count);
			condition.push_back(temp);
		}
	}

	void key_value(map<string,string> &col_table) {
		string table,key,value;
		str(key);
		Expect(31);
		if (la.kind == 3) {
			number(value);
		} else if (la.kind == 2) {
			strconst(value);
		} else SynErr(46);
		col_table.insert( make_pair( key,value)) ;
	}

	void strconst(string & xz) {
		Expect(2);
		string temp;
		temp.append(coco_string_create_char(t->val));
		xz.append(temp.substr(1,(strlen(temp.c_str())-2)));;
	}

	void strconst_list(vector<string> &list) {
		string temp;
		strconst(temp);
		list.push_back(temp);
	}

	void number_list(vector<string> &list) {
		string temp;
		number(temp);
		list.push_back(temp);
	}

	void con(vector<int> &x,vector<string> &a,vector<string> &b,vector<string> &c ,int &count) {
		if (la.kind == 8) {
			Get();
			Condition(x,a,b,c,count);
			Expect(10);
		} else if (la.kind == 1) {
			x.push_back(count);count++;
			str_list(a);
			r_list(b);
			if (la.kind == 3) {
				number_list(c);
			} else if (la.kind == 2) {
				strconst_list(c);
			} else SynErr(47);
		} else SynErr(48);
	}

	void r_list(vector<string> &list) {
		string temp;
		r(temp);
		list.push_back(temp);
	}

	void r(string &ab) {
		Expect(4);
		ab.append(coco_string_create_char(t->val));
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Sql();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, T,x,x,T, x,x,x,x}

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
			case 20: s = "\"char(\" expected"; break;
			case 21: s = "\"varchar(\" expected"; break;
			case 22: s = "\"drop\" expected"; break;
			case 23: s = "\"select\" expected"; break;
			case 24: s = "\"distinct\" expected"; break;
			case 25: s = "\"all\" expected"; break;
			case 26: s = "\"*\" expected"; break;
			case 27: s = "\"from\" expected"; break;
			case 28: s = "\"where\" expected"; break;
			case 29: s = "\"update\" expected"; break;
			case 30: s = "\"set\" expected"; break;
			case 31: s = "\"=\" expected"; break;
			case 32: s = "\"insert\" expected"; break;
			case 33: s = "\"into\" expected"; break;
			case 34: s = "\"values\" expected"; break;
			case 35: s = "\"delete\" expected"; break;
			case 36: s = "\"and\" expected"; break;
			case 37: s = "\"or\" expected"; break;
			case 38: s = "??? expected"; break;
			case 39: s = "invalid Sql"; break;
			case 40: s = "invalid Modify_data"; break;
			case 41: s = "invalid Select_command"; break;
			case 42: s = "invalid Insert_command"; break;
			case 43: s = "invalid Insert_command"; break;
			case 44: s = "invalid Name_type_pair"; break;
			case 45: s = "invalid type"; break;
			case 46: s = "invalid key_value"; break;
			case 47: s = "invalid con"; break;
			case 48: s = "invalid con"; break;

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
