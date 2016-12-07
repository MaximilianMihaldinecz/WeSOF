using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof.MVTManager
{
    class HtmlCommentTag
    {
        public bool? isWesofTag = null;
        public int? startIndex = null;
        public int? startTagendIndex = null;
    }

    class WesofHtmlTag : HtmlCommentTag
    {

        public string startTagString = null;

        public int? closingTagStartIndex = null;
        public int? closingTagEndIndex = null;

        public string contentBetweenTags = null;
        public string fullTagString = null;

        public bool? testingEnabled = null;
        public string objectType = null;
        public int? objectGroupID = null;

        public string hrefAsCTA = null;
    }

}
