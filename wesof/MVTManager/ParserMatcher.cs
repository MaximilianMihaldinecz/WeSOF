using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof.MVTManager
{
    class ParserMatcher
    {

        LinkedList<WesofHtmlTag> _TagsFound = null;
        private CommandLineInterface _CLI;
        private PostParser _PostParser = null;


        public ParserMatcher(CommandLineInterface cli)
        {
            _CLI = cli;
        }

        internal void ParseHtml(ref ServerResponse sRep )
        {
            CollectWeSOFTags(ref sRep);

            //Send the tags for post-processing
            if (_TagsFound != null)
            {                
                if (_PostParser == null)
                {
                    _PostParser = new PostParser(_CLI, ref _TagsFound, ref sRep);
                }
                _PostParser.SendTagsToMatchingMvtModules();
            }
        }

        private void CollectWeSOFTags(ref ServerResponse sRep)
        {   
 
            //Get the raw tags
            if(sRep.ModifiedBody != null && sRep.ModifiedBody != String.Empty)
            {
                GetAllWeSOFTags(ref sRep.ModifiedBody);
            }

            //Analyse tag attributes
            if(_TagsFound != null)
            {
                //Detect properties from the wesof tag headers
                DetectTagAttributes();  
            }
        }

        private void DetectTagAttributes()
        {
            char[] seperators = { ';' };

            LinkedListNode<WesofHtmlTag> iterator = _TagsFound.First;

            //Iterate through the LL
            while(iterator != null)
            {
                string[] headerOptions = iterator.Value.startTagString.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                if (headerOptions != null)
                {
                    iterator.Value.testingEnabled = GetTestableProperty(ref headerOptions);
                    iterator.Value.objectGroupID = GetObjectGroupID(ref headerOptions);
                    iterator.Value.objectType = GetObjectType(ref headerOptions);
                    iterator.Value.hrefAsCTA = GetHrefCTA(iterator.Value);
                }
                
                iterator = iterator.Next;
            }
        }

        private string GetHrefCTA(WesofHtmlTag value)
        {
            string searchFor = "href=\"";
            int startindex = value.fullTagString.IndexOf(searchFor) + searchFor.Length;
            if(startindex > -1)
            {
                int endindex = value.fullTagString.IndexOf("\"", startindex);
                if(endindex > -1)
                {
                    string result = value.fullTagString.Substring(startindex, endindex - startindex);

                    //href type CTA found
                    if(result.Length > 0)
                    {
                        //Is this tag stil relevant (testable), then add the href to the conversion triggers.                        
                       if(value.testingEnabled == true)
                        {
                            ConversionTrigger trigger = new ConversionTrigger();
                            trigger.Href = result;

                            if(value.objectType.ToUpper() == Configuration.MVT_MODULES_LIST[0].ToUpper())
                            {
                                trigger.TestType = TestTypesEnum.ADDTOCARTBUTTON;
                            }

                            ConversionManager.AddConversionTriggerIfnotExists(trigger);
                        }                       
                    }
                }
            }

            return null;
        }

        private bool? GetTestableProperty(ref string[] headerOptions)
        {
            foreach(string prop in headerOptions)
            {
                if(prop.ToUpper().Contains(Configuration.TAG_ISTESTABLE))
                {
                    if (prop.ToUpper().Contains("TRUE") || prop.ToUpper().Contains("1"))
                        return true;
                    else
                        return false;
                }
            }
            return null;
        }

        private int? GetObjectGroupID(ref string[] headerOptions)
        {
            foreach (string prop in headerOptions)
            {
                if (prop.ToUpper().Contains(Configuration.TAG_OBJECTGROUPID))
                {
                    char[] sepearator = { ':' };
                    string[] value = prop.Split(sepearator, StringSplitOptions.RemoveEmptyEntries);

                    if(value.Length == 2)
                    {
                        int? result = null;
                        try
                        {
                            result = int.Parse(value[1]);
                        } catch
                        {
                            if (Configuration.IsDebugModeOn)
                            {
                                _CLI.Out("Error. Cannot convert objectGroupID to number from string.");
                            }
                            return null;
                        }
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        private string GetObjectType(ref string[] headerOptions)
        {
            foreach (string prop in headerOptions)
            {
                if (prop.ToUpper().Contains(Configuration.TAG_OBJECTTYPE))
                {
                    char[] sepearator = { ':' };
                    string[] value = prop.Split(sepearator, StringSplitOptions.RemoveEmptyEntries);

                    if(value.Length == 2)
                    {
                        return value[1];
                    }
                    else
                    {
                        if (Configuration.IsDebugModeOn)
                        {
                            _CLI.Out("Error. Cannot identify objectType.");
                        }
                        return null;
                    }
                }
            }
            return null;
        }

        private void GetAllWeSOFTags(ref string htmlText)
        {
            HtmlCommentTag temp = null;
            int startPos = 0;
            do
            {
                temp = GetNextHTMLWeSofTag(startPos, ref htmlText);                
                if (temp != null)
                {
                    //Something has been found...
                    if (temp.isWesofTag == true)
                    {
                        //And this was a weSOF section
                        if (_TagsFound == null)
                        {
                            _TagsFound = new LinkedList<WesofHtmlTag>();
                        }

                        //Adding tag to the list
                        _TagsFound.AddLast((WesofHtmlTag)temp);

                        //Continue searching from the end position                        
                        if(((WesofHtmlTag)temp).closingTagEndIndex != null)
                        {
                            startPos = (int)((WesofHtmlTag)temp).closingTagEndIndex;
                        }
                        else
                        {
                            //A tag found without end... break out from the loop
                            if (Configuration.IsDebugModeOn)
                            {
                                _CLI.Out("Error. A WeSOF comment tag found without closing end. Breaking up searching of WeSOF tags.");
                            }
                            break;
                        }
                        
                    } 
                    else
                    {
                        //And this was just a general HTML tag.
                        //Continue searching after that. 
                        if(temp.startTagendIndex != null)
                        {
                               startPos = (int)temp.startTagendIndex;
                        }
                        else
                        {
                            //A tag found without end... break out from the loop
                            if(Configuration.IsDebugModeOn)
                            {
                                _CLI.Out("Error. A html comment tag found without closing end. Breaking up searching of WeSOF tags.");
                            }
                            break;
                        }
                    }      
                }                

            } while (temp != null);
        }

        private HtmlCommentTag GetNextHTMLWeSofTag(int startPos, ref string htmlText)
        {
            WesofHtmlTag result = null;

            //Finds the next HTML comment start
            int start = FindHtmlCommentTag(startPos, true, ref htmlText);
            int end = -1;
            //Finds the comment end
            if (start > -1)
            {
                end = FindHtmlCommentTag(start, false, ref htmlText);
            }
          

            //If tag found, checks if it is a WESOF start tag
            if(end > -1)
            {
                if(IsHtmlTagWeSofTag(ref htmlText,start,end,true))
                {
                    //It was a WeSOF start tag.
                    result = new WesofHtmlTag();
                    result.isWesofTag = true;
                    result.startIndex = start;
                    result.startTagendIndex = end;
                    result.startTagString = htmlText.Substring(start, end - start);

                    //Search for the closing tag
                    int closingStart;
                    int closingEnd;
                    bool isWesofTagStop = false;

                    //Search closing tags
                    int searchForEndFrom = end;
                    do
                    {
                        //Find first opening html comment tag
                        closingStart = FindHtmlCommentTag(searchForEndFrom, true, ref htmlText);
                        if(closingStart > -1)
                        {
                            //Find the end of the comment tag
                            closingEnd = FindHtmlCommentTag(closingStart, false, ref htmlText);
                            if(closingEnd > -1)
                            {
                                //Check if this was WeSOF closing tag
                                isWesofTagStop = IsHtmlTagWeSofTag(ref htmlText, closingStart, closingEnd, false);

                                if(isWesofTagStop)
                                {
                                    result.closingTagStartIndex = closingStart;
                                    result.closingTagEndIndex = closingEnd;
                                    result.fullTagString = htmlText.Substring(start, closingEnd - start);                                    
                                    result.contentBetweenTags = htmlText.Substring(end, closingStart - end);

                                    return result;
                                }
                                else
                                {
                                    //Start look for a new HTML tag from the last html comment end.
                                    searchForEndFrom = closingEnd;
                                }                               
                            }
                            else
                            {
                                //No closing end found for an html comment. That's unlikely.
                                if (Configuration.IsDebugModeOn)
                                {
                                    _CLI.Out("Error. Cannot find closing for an HTML comment. Maybe the HTML file is invalid.");
                                }
                                return null;
                            }                            
                        }                        
                    } while (closingStart > -1 && !isWesofTagStop);


                    if(isWesofTagStop)
                    {
                        return result;
                    }
                } 
                else
                {
                    //The found tag was a HTML comment, but not a WeSOF tag.
                    HtmlCommentTag commentFoundOnly = new HtmlCommentTag();
                    commentFoundOnly.isWesofTag = false;
                    commentFoundOnly.startIndex = start;
                    commentFoundOnly.startTagendIndex = end;
                    return commentFoundOnly;
                }               
            }

            return null;
        }

        private bool IsHtmlTagWeSofTag(ref string htmlText, int startIndex, int endIndex, bool lookForStartTag)
        {
            if(lookForStartTag)
            {
                return htmlText.IndexOf(Configuration.WeSOFHtmlTagStartString, startIndex, endIndex - startIndex) > -1;
            }
            else
            {
                return htmlText.IndexOf(Configuration.WeSOFHtmlTagStopString, startIndex, endIndex - startIndex) > -1;
            }            
        }

        private int FindHtmlCommentTag(int startPos, bool isStartingTag, ref string htmlText)
        {
            string pattern;
            if (isStartingTag)
                pattern = @"<!--";
            else
                pattern = @"-->";

            int result = htmlText.IndexOf(pattern,startPos);

            //Offsetting index if closing tag was searched
            if (!isStartingTag && result > -1)
                result += pattern.Length;

            return result;            
        }

        
    }
}
