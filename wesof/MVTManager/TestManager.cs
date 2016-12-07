using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wesof.MVTModules;
using wesof.MVTModules.AddToCartButton;

namespace wesof.MVTManager
{
    class TestManager
    {

        CommandLineInterface _CLI;

        public TestManager(CommandLineInterface cli)
        {
            _CLI = cli;
        }


        public void HandleAddToCartCTATags(WesofHtmlTag[] Tags, ref ServerResponse sRep)
        {
            
            //Check if the clients currently assigned MVT's are addtocart tests
            Variation[] relevantTestAssignments = MVTHistory.MatchLiveMVTWithTestType(sRep.Cookie.ModifiedMVTAssignments, TestTypesEnum.ADDTOCARTBUTTON);
            if(relevantTestAssignments !=null)
            {
                //Todo, the client is already assigned to live AddToCartButton test(s) 
                for (int i = 0; i < relevantTestAssignments.Length; i++)
                {
                    relevantTestAssignments[i].ApplyModification(ref Tags, ref sRep);
                }

                return;
            }


            //Check if there is already a live mvt that experiments with the add to cart.
            int assignToMVTId = MVTHistory.GetSuitableLiveMVTforNewGUIDAssignment(TestTypesEnum.ADDTOCARTBUTTON);
            int assignToVariation;
            if (assignToMVTId == -1)
            {
                //No suitable live MVT found. Generate a new one.
                assignToMVTId = CreateNewAddToCartMVT();
                                
            }
            
            //Check if a test is available
            if(assignToMVTId > -1)
            {
                assignToVariation = AssignGuidToMVT(assignToMVTId, sRep.Cookie.Wesof_GUID, ref sRep);
                if(assignToVariation > -1)
                {
                    Variation v = MVTHistory.GetVariant(assignToMVTId, assignToVariation);
                    if(v != null)
                    {
                        v.ApplyModification(ref Tags, ref sRep);
                    } 
                    else
                    {
                        if (Configuration.IsDebugModeOn)
                            _CLI.Out("Error. Expected variation object, returned null.", true, false);
                    }                       
                }
            }
            else
            {
                //Check if all the possible variations tested
                if(AddToCartButton.IsAllVariationTested())
                {
                    //Let's apply the winning variant.
                    AddToCartButton.ApplyChangesBasedOnLatestWinner(ref Tags, ref sRep);
                }
            }

            _CLI.RefreshStatDashboard();
        }



        private int AssignGuidToMVT(int mvtId, string guid, ref ServerResponse sRep)
        {
            int variantId = MVTHistory.AssignGuidToMVTEvenly(mvtId, guid);
            sRep.Cookie.AssignResponseToMVT(mvtId, variantId, ref sRep);

            if(variantId == -1)
            {
                if (Configuration.IsDebugModeOn)
                    _CLI.Out("Error. The GUID assigned to variant id -1.", true, false);
            }

            return variantId;           
        }


        private int CreateNewAddToCartMVT()
        {
            int mvtid = -1;
            VariationType testVariant = AddToCartButton.GetNextUntestedVariant();

            if(testVariant != null)
            {
                mvtid = MVTHistory.CreateNewLiveMVT(TestTypesEnum.ADDTOCARTBUTTON);
                MVTHistory.AddNewTestVariantToMVT(mvtid, ref testVariant);
            }
            else
            {
                //Could not create new MVT as could not produce new test variant. 
                if(AddToCartButton.IsAllVariationTested() == false)
                {
                    //There are still untested variants. This must be an error.
                    if (Configuration.IsDebugModeOn)
                        _CLI.Out("Error while trying to create new MVT.", true, false);
                }               
                
                return -1;
            }            

            return mvtid;
        }



        /*
        private void TEMP_REPLACER(WesofHtmlTag[] Tags, ref ServerResponse sRep)
        {
            int id = 1;

            foreach (WesofHtmlTag tag in Tags)
            {
                AddToCartButtonVariation nButton = AddToCartButton.GetVariant(id, ref tag.fullTagString);

                if(nButton != null)
                {
                    sRep.ModifiedBody = sRep.ModifiedBody.Replace(nButton.UnModified, nButton.Modified);
                }

                id++;      
            }    
        }*/



        

    }
}
