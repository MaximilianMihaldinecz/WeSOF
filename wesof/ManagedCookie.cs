using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof
{
    class ManagedCookie
    {
        private static int NextAvailableGuid = 1;

        private bool clientHasWesofGUID = false;
        private bool clientHasMVTAssignment = false;

        private string originalRequestCookie = null;
        private string[] originalRequestCookieSplitted = null;
        private string[] originalMVTAssignments = null;
        private string originalMVTAssignmentsRAW = null;

        private string modifiedMVTAssignment = null;
        private string[] modifiedMVTAssignmentsSplitted = null;
        
        private string WesofGUID = null;

        public string[] OriginalMVTAssignments
        {
            get
            {
                return originalMVTAssignments;
            }            
        }

        public string Wesof_GUID
        {
            get
            {
                return WesofGUID;
            }
        }

        public string[] ModifiedMVTAssignments
        {
            get
            {
                return modifiedMVTAssignmentsSplitted;
            }

            set
            {
                modifiedMVTAssignmentsSplitted = value;
            }
        }

        public void SetOriginalRequestCookie(string plainCookieText)
        {
            if(plainCookieText != null)
            {
                originalRequestCookie = plainCookieText;
                //Removes the initial "Cookie:" string and splits up the cookies at the ; character. Also removes empty spaces
                originalRequestCookieSplitted = originalRequestCookie.Remove(0,7).Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                CheckIfRequestCookieHasGuid();
                CheckIfRequestCookieHasMVTHistory();
            }
        }


        private void CheckIfRequestCookieHasGuid()
        {
            for(int i = 0; i<originalRequestCookieSplitted.Length; i++)
            {
                if(originalRequestCookieSplitted[i].Contains(Configuration.WesofCookieGuidName))
                {
                    clientHasWesofGUID = true;
                    WesofGUID = originalRequestCookieSplitted[i].Replace(Configuration.WesofCookieGuidName + "=", "");
                }
            }
        }

        internal void AssignResponseToMVT(int mvtId, int variantId, ref ServerResponse sRep)
        {
            string tmp = "Set-Cookie: ";

            if(modifiedMVTAssignment != null)
            {
                tmp += modifiedMVTAssignment + ",";
            }
            else
            {
                tmp += Configuration.WesofCookieMVTAssignmentsName + "=";
            }


            tmp += mvtId.ToString() + "." + variantId.ToString() + "; ";
            tmp += "path=/" + "; ";
            DateTime expDate = DateTime.UtcNow.AddYears(5);
            tmp += "expires=" + expDate.ToString("r") ;


            int insertionPoint = sRep.ModifiedHeader.LastIndexOf("\r\n\r\n");
            sRep.ModifiedHeader = sRep.ModifiedHeader.Insert(insertionPoint, "\r\n" + tmp);
        }

        private void CheckIfRequestCookieHasMVTHistory()
        {
            for (int i = 0; i < originalRequestCookieSplitted.Length; i++)
            {
                if (originalRequestCookieSplitted[i].Contains(Configuration.WesofCookieMVTAssignmentsName))
                {
                    clientHasMVTAssignment = true;

                    originalMVTAssignmentsRAW = originalRequestCookieSplitted[i];
                    modifiedMVTAssignment = originalMVTAssignmentsRAW;

                    string tmp = originalRequestCookieSplitted[i].Replace(Configuration.WesofCookieMVTAssignmentsName + "=", "");
                    originalMVTAssignments = tmp.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    //clone the original array to the modified one
                    modifiedMVTAssignmentsSplitted = new string[originalMVTAssignments.Length];
                    for(int j = 0; j<OriginalMVTAssignments.Length; j++)
                    {
                        modifiedMVTAssignmentsSplitted[j] = originalMVTAssignments[j];
                    }
                }
            }
        }

        internal void EnsureGUIDisPresent(ref ServerResponse sRep)
        {
            if(clientHasWesofGUID == true && WesofGUID !=null)
            {
                //return if already have guid
                return;
            }

            string newGuidString = GenerateGuidString();
            int insertionPoint= sRep.ModifiedHeader.LastIndexOf("\r\n\r\n");

            sRep.ModifiedHeader = sRep.ModifiedHeader.Insert(insertionPoint, "\r\n" + newGuidString);
        }

        private string GenerateGuidString()
        {
            WesofGUID = NextAvailableGuid.ToString();
            string tmp = "Set-Cookie: ";
            tmp += Configuration.WesofCookieGuidName + "=" + NextAvailableGuid.ToString() + "; ";
            tmp += "path=/" + "; ";
            DateTime expDate = DateTime.UtcNow.AddYears(5);
            tmp += "expires=" + expDate.ToString("r") + "; ";            
            NextAvailableGuid++;
            return tmp;
        }


        public void CleanUpModifiedMVTList()
        {
            List<string> nAssignedMVTs = new List<string>();

            foreach(string s in modifiedMVTAssignmentsSplitted)
            {
                if(s != string.Empty)
                {
                    nAssignedMVTs.Add(s);
                }
            }

            if(nAssignedMVTs.Count == 0)
            {
                modifiedMVTAssignment = null;
                ModifiedMVTAssignments = null;
                return;
            }
            else
            {
                ModifiedMVTAssignments = nAssignedMVTs.ToArray();
                
                modifiedMVTAssignment = Configuration.WesofCookieMVTAssignmentsName + "=";
                for (int i = 0; i<ModifiedMVTAssignments.Length; i++)
                {
                    modifiedMVTAssignment += ModifiedMVTAssignments[i];

                    if(i<ModifiedMVTAssignments.Length-1)
                    {
                        modifiedMVTAssignment += ",";
                    }
                }

            }

        }

    }
}
