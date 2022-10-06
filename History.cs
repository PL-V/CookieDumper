using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookiesDumper
{
	class History
	{
        public string Url;
        public string Title;
        public int VisitCount;
        public History(string Url,string Title,int VisitCount) {
            this.Url = Url;
            this.Title = Title;
            this.VisitCount = VisitCount;
        
        }
                            
	}
}
