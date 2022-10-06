using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookiesDumper
{
	class Cookie
	{



        public string HostKey;
        public string Name;
        public string Value;
        public string DecryptedCookieValue;
        public string Path;
        public long ExpiresUTC;

        public Cookie(string HostKey,string Name,string Value,string DecryptedCookieValue, string Path, long ExpiresUTC) {

            this.HostKey = HostKey;
            this.Name = Name;
            this.Value = Value;
            this.DecryptedCookieValue = DecryptedCookieValue;
            this.Path = Path;
            this.ExpiresUTC = ExpiresUTC;
        
        }


	}
}
