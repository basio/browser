using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace browser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //webBrowser1.Navigate("");
            state = 0;

            string url = @"C:\Users\khalefa\Documents\Visual Studio 2012\Projects\browser\mis.html";
            //url = @"C:\tmp\a.html";
            url=@"http://mis.alexu.edu.eg/test/ui/loginpage.aspx";
            webBrowser1.Navigate(url);
            webBrowser1.Document.Encoding = "utf-8";           
        }
        void loginpage(HtmlDocument doc)
        {
            Uri u = doc.Url;
            System.Diagnostics.Debug.WriteLine(u);

            HtmlElement name = doc.GetElementById("txtUsername");

            if (name != null) name.SetAttribute("value", "basiony");
            HtmlElement pass = doc.GetElementById("txtPassword");
            if (pass != null) pass.SetAttribute("value", "mis123456");

            var button = doc.GetElementById("btnEnter");
            if (button != null)
                button.InvokeMember("click");

        }
        int state = -1;
        private void Addmember()
        {
            if (state == 2)
            {
                string url = "http://mis.alexu.edu.eg/test/UI/StaffAffairs/StaffBasicData/StfMemberMainPage.aspx";
                webBrowser1.Navigate(url);
            }

        }
        List<HtmlElement> elements = new List<HtmlElement>();
        void getlocations(HtmlDocument doc)
        {
            StreamWriter sw = new StreamWriter(@"c:/tmp/loc.txt");

            foreach (HtmlElement e in doc.All)
            {
                if (e.Children.Count != 0) continue;
                string id = "";
                id = e.GetAttribute("id");
                string parent = "null parent";
                if (e.Parent != null) parent = e.Parent.TagName;
                string text = "";
                if (e.InnerText != null) text = e.InnerText;
                elements.Add(e);
                sw.WriteLine(e.TagName + "\t" + id + text + "\t" + Helper.AbsRectangle(e));
            }
            sw.Close();
        }

        //find labels for elements
        //would try 
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(state >3) return;
            state++;
            HtmlDocument doc = webBrowser1.Document;
            Uri u = doc.Url;
            if (u.ToString().ToLower().Contains("login"))
            {
                loginpage(doc);
                state = 1;
            }
            else
            {
                var ssn = doc.GetElementById("ctl00_cntphmaster_txtStfNationalIdNum");
                if (ssn == null)
                {
                    Addmember();
                    return;
                }
                ssn.SetAttribute("Value", "25101260200718");
                var search = doc.GetElementById("ctl00_cntphmaster_btnGo");
                search.InvokeMember("click");
                // doc.GetElementFromPoint();
                //HtmlElement h = search;
                // h.ClientRectangle
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Net.WebClient wc = new System.Net.WebClient();

            System.IO.Stream stream = wc.OpenRead(@"C:\Users\khalefa\Documents\Visual Studio 2012\Projects\browser\mis.html");
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            string s = reader.ReadToEnd();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(s);
            StreamWriter sw = new StreamWriter("c:/tmp/log.txt");
            var all = doc.DocumentNode.Descendants("table");
            foreach (var x in all)
            {
                Debug.WriteLine(x.NodeType + "\t" + x.InnerText + "--" + x);
                var href = x.Attributes["href"];
                if (href != null)
                    sw.WriteLine(x.Name + ":" + x.NodeType + "\t" + x.InnerText + "--" + href.Value);
                else
                    sw.WriteLine(x.Name + ":" + x.NodeType + "\t" + x.InnerText + "--" + "null");
                sw.WriteLine("------------------------------------------------------");
                sw.WriteLine("------------------------------------------------------");
                //Debug.WriteLine("------------------------------------------------------");
                //Debug.WriteLine("------------------------------------------------------");
            }
            sw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            webBrowser1.Document.Encoding = "utf-8";
            getlocations(webBrowser1.Document);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HtmlElement  e1= (HtmlElement) elements[6];
            HtmlElement  e2= (HtmlElement) elements[10];
            HtmlElement o = Helper.getCommonAnsector(e1, e2);
        }
    }
}
