using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wesof.MVTModules;
using wesof.MVTModules.AddToCartButton;

namespace wesof.MVTManager
{
    class MultiVariateTest
    {
        private int id = -1;
        private TestTypesEnum testType; 
        private List<Variation> variants = new List<Variation>();
        private int AssignedGuidsTotal;

        private MVTResult Result = null;

        private bool isActive = false;
        private bool isVariantWinner = false;
        private int winningVariantIndex = -1;

        private DateTime? LaunchDateTime = null;
        private DateTime? CompletionDate = null;

        TestResultType ResultType;


        public TestResultType GetResultType()
        {
            return ResultType;
        }

        public MultiVariateTest(int mvtid, TestTypesEnum tType)
        {
            id = mvtid;
            testType = tType;
            isActive = true;
            LaunchDateTime = DateTime.UtcNow;
            ResultType = TestResultType.Live;
            AssignedGuidsTotal = 0;

            Variation control = control = new Variation(0, true);            
            if (tType == TestTypesEnum.ADDTOCARTBUTTON)
            {                
                control.SetVariationType(AddToCartButton.GetCurrentWinnderControl());               
            }
            
            
            variants.Add(control);
        }

        public void AddTestVariant(ref VariationType testVariant)
        {
            int vid = variants.Count;
            Variation variant = new Variation(vid, false);
            variant.SetVariationType(testVariant);

            variants.Add(variant);
        }

        /// <summary>
        /// Finalises a test that reached completion.
        /// </summary>
        private void FinishTest()
        {
            if(isActive == true)
            {
            
                CompletionDate = DateTime.Now;
                isActive = false;

                //Picking winning variant based on highest mean (and reached significance)
                //TODO, this implementation works only for A/B. For MVT (multi variants, a loop is needed
                //to select the most promising variant.

                //Did it reach significance
                if(Result.ReachedSignificance[0] == true)
                {
                    //Is the control or the test variant has higher mean. 
                    int winnerVariantId = -1;
                    if (Result.Means[0] > Result.Means[1])
                    {
                        winnerVariantId = 0;
                        ResultType = TestResultType.Negative;
                    }                        
                    else
                    {
                        winnerVariantId = 1;
                        ResultType = TestResultType.Positive;
                    }
                        


                    if (testType == TestTypesEnum.ADDTOCARTBUTTON)
                    {
                        //Set the new winner control variant.  
                        AddToCartButton.SetNewWinnerControl((int)variants[winnerVariantId].GetVariationType().id);
                    }
                }
                else
                {
                    //If test did not reach significance, then consider it abandonned. 
                    ResultType = TestResultType.Inconclusive;                    
                }
                

                /*
                int highestmeanIndex = -1;
                for (int i = 0; i < Result.ReachedSignificance.Length; i++)
                {
                
                    if(Result.ReachedSignificance[i] == true)
                    {
                        if(highestmeanIndex == -1)
                        {
                            highestmeanIndex = i;
                        }
                        else
                        {
                            //Is the test variant better than the control
                            if(Result.Means[0] > Result.Means[i+1])
                            {
                                highestmeanIndex = i + 1;
                            }
                        }
                    }

                }*/
                

            }
        }
           
        public void UpdateTestResult()
        {
            Result = MVTAnalyser.AnalyseTest(this);

            //Check if the test is reached significance
            //If any of the variant reached significance, the finalise test
            if(Result != null)
            {
                if(Result.IsAnyReachedSignificance || (AssignedGuidsTotal > Result.GuidLimitBeforeAbandon))
                {
                    FinishTest();
                }
            }
        }

        internal int GetNumberOfVariants()
        {
            if(variants!=null)
            {
                return variants.Count;
            }
            return -1;
        }

        internal int AssignGuidEvenly(string guid)
        {
            //If a guid is already assigned, just return the currently assigned variant
            int isAssigned = IsAlreadyAssigned(guid);
            if(isAssigned > -1)
            {
                return isAssigned;
            }


            int leastAssignedVariant = 0;
            for(int i = 1; i < variants.Count; i++ )
            {
                if(variants[i].GetAssignedGuidCount() < variants[leastAssignedVariant].GetAssignedGuidCount())
                {
                    leastAssignedVariant = i;
                }
            }

            variants[leastAssignedVariant].AssignGuid(guid);
            AssignedGuidsTotal++;


            //Do test analysis. This has a performance penalty. 
            if(Configuration.IsAnalysisRunWithNewGuid)
            {
                UpdateTestResult();
            }


            return leastAssignedVariant;
        }


        private int IsAlreadyAssigned(string guid)
        {
            int result;
            for(result = 0; result < variants.Count; result++)
            {
                if(variants[result].IsGUIDAssigned(guid))
                {
                    return result;
                }
            }

            return -1;
        }

        internal Variation GetVariantWithGuid(string guid)
        {
            for (int i = 0; i < variants.Count; i++)
            {
                if (variants[i].IsGUIDAssigned(guid))
                    return variants[i];
            }
            return null;
        }

        public Variation GetVariant(int index)
        {
            try
            {
                return variants[index];
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public bool IsActive
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
            
        }

        internal TestTypesEnum TestType
        {
            get
            {
                return testType;
            }           
        }

        public DateTime? GetLaunchDateTime
        {
            get
            {
                return LaunchDateTime;
            }
                        
        }

        internal MVTResult GetResult
        {
            get
            {
                return Result;
            }
            
        }

        public DateTime? GetCompletionDate
        {
            get
            {
                return CompletionDate;
            }

            set
            {
                CompletionDate = value;
            }
        }

        public int GetAssignedGuidNumber(int variantIndex)
        {
            if(variantIndex < variants.Count)
            {
                return variants[variantIndex].GetAssignedGuidCount();
            }
            return 0;
        }

        internal List<byte> GetVariantConversionValuesAsNumbers(int variantIndex)
        {
            if (variantIndex < variants.Count)
            {
                return variants[variantIndex].GetGuidCVRResultsAsNumbers();
            }
            return null;
        }

    }
}
