using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace browser
{
    class Helper
    {
        //get abosulte location for HtmlElement
       public static Rectangle AbsRectangle(HtmlElement e){
            int X=e.OffsetRectangle.X;
            int Y = e.OffsetRectangle.Y;
            Size s = e.OffsetRectangle.Size;
            while (e.OffsetParent != null)
            {
                
                e = e.OffsetParent;
                X += e.OffsetRectangle.X;
                Y += e.OffsetRectangle.Y;
            }
            return new Rectangle(X, Y, s.Width, s.Height);
        }

       public static HtmlElement getCommonAnsector(HtmlElement e1, HtmlElement e2)
       {
           //get the parents of the first element
           HashSet<HtmlElement> p1 = new HashSet<HtmlElement>();
           p1.Add(e1);
           while (e1.Parent != null)
           {
               e1 = e1.Parent;
               p1.Add(e1);               
           }

           
           for (; ; )
           {
               if (p1.Contains(e2)) return e2;
               e2 = e2.Parent;
               if (e2 == null) return null;
           }
        }
        
    }
}
