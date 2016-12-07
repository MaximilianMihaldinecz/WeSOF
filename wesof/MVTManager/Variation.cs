using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wesof.MVTModules;
using wesof.MVTModules.AddToCartButton;

namespace wesof.MVTManager
{
    class Variation
    {
        int id = -1;
        bool isControl = false;
        VariationType VariationType = null;

        List<GUIDStat> AssignedGUIDs = new List<GUIDStat>();

        public int Id
        {
            get
            {
                return id;
            }
        }

        public Variation (int vid, bool control)
        {
            id = vid;
            isControl = control;
            
        }

        public void SetVariationType(VariationType vtype)
        {
            VariationType = vtype;
        }

        class GUIDStat
        {
            public GUIDStat(string guid)
            {
                _Guid = guid;
            }

            public string _Guid;
            public bool converted = false;
            public int conversionCounter = 0;
            public int view = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>The previous conversion status of that guid</returns>
        public bool ConvertGuid(string guid)
        {
            int index = IndexByGuid(guid);
            if(index>-1)
            {
                bool result = AssignedGUIDs[index].converted;
                AssignedGUIDs[index].converted = true;
                AssignedGUIDs[index].conversionCounter++;

                return result;
            }

            return false;
        }

        internal void ApplyModification(ref WesofHtmlTag[] tags, ref ServerResponse sRep)
        {
            //Track visit count
            if(sRep.Cookie.Wesof_GUID != null)
            {
                int index = IndexByGuid(sRep.Cookie.Wesof_GUID);
                if(index >-1)
                {
                    AssignedGUIDs[index].view++;
                }
            }


            /*
            if (isControl)
                return;
            */

            if(VariationType.Type == TestTypesEnum.ADDTOCARTBUTTON)
            {
                AddToCartButtonModification(ref tags, ref sRep);
            }
        }


        private void AddToCartButtonModification(ref WesofHtmlTag[] tags, ref ServerResponse sRep)
        {
            foreach (WesofHtmlTag tag in tags)
            {
                AddToCartButtonVariation nButton = AddToCartButton.GetVariant((int)VariationType.id, ref tag.fullTagString);
                if (nButton != null)
                {
                    sRep.ModifiedBody = sRep.ModifiedBody.Replace(nButton.UnModified, nButton.Modified);
                }                
            }
        }


        public int GetAssignedGuidCount()
        {
            return AssignedGUIDs.Count;
        }

        internal bool IsGUIDAssigned(string guidToFind)
        {            
            for (int i = 0; i < AssignedGUIDs.Count; i++)
            {
                if(AssignedGUIDs[i]._Guid == guidToFind)
                {
                    return true;
                }
            }
            return false;
        }

        private int IndexByGuid(string guidToFind)
        {
            for(int i = 0; i < AssignedGUIDs.Count; i++)
            {
                if (AssignedGUIDs[i]._Guid == guidToFind)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void AssignGuid(string guid)
        {
            AssignedGUIDs.Add(new GUIDStat(guid));
        }


        public int GetGuidCount()
        {
            if(AssignedGUIDs != null)
            {
                return AssignedGUIDs.Count;
            }
            return 0;
        }

        internal List<byte> GetGuidCVRResultsAsNumbers()
        {
            if (AssignedGUIDs != null)
            {
                List<byte> result = new List<byte>(AssignedGUIDs.Count);

                for (int i = 0; i < AssignedGUIDs.Count; i++)
                {
                    if(AssignedGUIDs[i].converted)
                    {
                        result.Add(1);
                    }
                    else
                    {
                        result.Add(0);
                    }
                }

                return result;             
            }
            return null;
        }


        public VariationType GetVariationType()
        {
            return VariationType;
        }


    }
}
