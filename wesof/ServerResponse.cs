using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof
{
    class ServerResponse
    {
        public byte[] Header = null;
        public string StrHeader = null;
        public string ModifiedHeader = null;
        public byte[] Body = null;
        public string ModifiedBody = null;
        public bool? isHtmlTextContent = null;
        public byte[] MergedModified = null;

        public ManagedCookie Cookie = null;
    }
}
