using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wesof.MVTModules.AddToCartButton;

namespace wesof.MVTManager
{
    class Stats
    {


        public static string TestHistoryAsCSV()
        {
            string result = "";
            MultiVariateTest[] allTests = MVTHistory.GetAllTests();

            if(allTests != null && allTests.Length > 0)
            {
                result =
                    "Test ID," + "Status," + "Launched," + "Finished," + "Type," + "Control variant," + "Test variant,"
                    + "CTR Visitors," + "TST Visitors," + "CTR Converted," + "TST Converted," + "CTR Conversion %," + "TST Conversion %," + "CTR Std Dev," + "TST Std Dev,"
                    + "T-Score," + "Degree of Freedom," + "Next Degree of Freedom," + "Required P for next DFree"
                    + "\n";

                for(int i = 0; i < allTests.Length; i++)
                {
                    MVTResult TResult = allTests[i].GetResult;

                    //Test ID
                    result += allTests[i].Id.ToString() + ",";

                    //Status
                    switch(allTests[i].GetResultType())
                    {
                        case TestResultType.Inconclusive:
                            result += "Inconclusive,";
                            break;
                        case TestResultType.Live:
                            result += "Live,";
                            break;
                        case TestResultType.Negative:
                            result += "Negative,";
                            break;
                        case TestResultType.Positive:
                            result += "Positive,";
                            break;
                        case TestResultType.None:
                            result += "None,";
                            break;
                    }

                    //Launched
                    DateTime? launched = allTests[i].GetLaunchDateTime;
                    if(launched != null)
                    {
                        result += launched.ToString() + ",";
                    }
                    else
                    {
                        result += ",";
                    }

                    //Finished
                    DateTime? finished = allTests[i].GetCompletionDate;
                    if(finished != null)
                    {
                        result += finished.ToString() + ",";
                    }
                    else
                    {
                        result += ",";
                    }

                    //Type
                    if(allTests[i].TestType == TestTypesEnum.ADDTOCARTBUTTON)
                    {
                        result += "Add to Cart Btn,";
                    }
                    else
                    {
                        result += ",";
                    }

                    //Control variant description
                    AddToCartButtonVariation ctrVar = (AddToCartButtonVariation)allTests[i].GetVariant(0).GetVariationType();   
                    if (ctrVar.colorPalette == null && ctrVar.addToCartTextCopies == null)
                    {
                        result += "No changes";
                    }
                    else
                    {
                        if (ctrVar.colorPalette != null)
                            result += "Colour: " + ctrVar.colorPalette + "; ";

                        if (ctrVar.addToCartTextCopies != null)
                            result += "Copy change: " + ctrVar.addToCartTextCopies;
                    }
                    result += ",";

                    //Test variant description
                    AddToCartButtonVariation tstVar = (AddToCartButtonVariation)allTests[i].GetVariant(1).GetVariationType();
                    if (tstVar.colorPalette == null && tstVar.addToCartTextCopies == null)
                    {
                        result += "No changes";
                    }
                    else
                    {
                        if (tstVar.colorPalette != null)
                            result += "Colour: " + tstVar.colorPalette + "; ";

                        if (tstVar.addToCartTextCopies != null)
                            result += "Copy change: " + tstVar.addToCartTextCopies;
                    }
                    result += ",";


                    
                    if(TResult != null)
                    {                        
                        if (TResult.SampleSize.Length > 1 &&
                            TResult.Means.Length > 1 &&
                            TResult.ConvertedSize.Length > 1 &&
                            TResult.STDevs.Length > 1)
                        {
                            //CTR visitors
                            result += TResult.SampleSize[0].ToString() + ",";

                            //TST visitors
                            result += TResult.SampleSize[1].ToString() + ",";

                            //CTR converted
                            result += TResult.ConvertedSize[0].ToString() + ",";

                            //TST converted
                            result += TResult.ConvertedSize[1].ToString() + ",";

                            //CTR conversion
                            result += TResult.Means[0].ToString() + ",";

                            //TST conversion
                            result += TResult.Means[1].ToString() + ",";

                            //CTR std dev
                            result += TResult.STDevs[0].ToString() + ",";

                            //TST std dev
                            result += TResult.STDevs[1].ToString() + ",";                            

                            //T-score
                            result += TResult.TScores[0].ToString() + ",";

                            //Degree of freedom
                            result += TResult.DFree[0].ToString() + ",";

                            //Next degree of freedom
                            result += TResult.NextDFree[0].ToString() + ",";

                            //Required DFree
                            result += TResult.ReqPVal[0].ToString();

                        }
                    }

                    result += "\n";

                }
            }
            else
            {
                result = "No tests launched yet.";
            }

            return result;
        }


        public static void PrintDashboard(CommandLineInterface _CLI)
        {
            int TotalMVTs = MVTHistory.GetMVTCount(false);
            int LiveMVTs = MVTHistory.GetMVTCount(true);
            int CompletedMVTs = TotalMVTs - LiveMVTs;
            int PositiveMVTs = MVTHistory.GetMVTCount(TestResultType.Positive);
            int NegativeMVTs = MVTHistory.GetMVTCount(TestResultType.Negative);
            int InconclusiveMVTs = MVTHistory.GetMVTCount(TestResultType.Inconclusive);

            MultiVariateTest[] LiveTests = MVTHistory.GetLiveTests();

            string result = "************************** OVERALL TESTING SUMMARY *************************\n\n";
            result += "Total MVTs launched: " + TotalMVTs.ToString()
                + " (" + CompletedMVTs.ToString() + " completed, "
                + LiveMVTs.ToString() + " currently live)\n";

            result += "Test results:\t" + PositiveMVTs.ToString() + " Positive\t"
                + NegativeMVTs.ToString() + " Negative\t"
                + InconclusiveMVTs.ToString() + " Inconclusive (abandoned) \n\n";

            if(LiveTests != null && LiveTests.Length > 0)
            {
                result += "******************************** LIVE TESTS ********************************\n";

                for(int i = 0; i < LiveTests.Length; i++)
                {
                    int testId = LiveTests[i].Id;
                    TestTypesEnum type = LiveTests[i].TestType;
                    string typeStr = "";
                    if (type == TestTypesEnum.ADDTOCARTBUTTON)
                        typeStr = "Add to Cart Btn";

                    DateTime? launched = LiveTests[i].GetLaunchDateTime;
                    int variantsNumber = LiveTests[i].GetNumberOfVariants();

                    MVTResult TResult = LiveTests[i].GetResult;



                    result += "[MVT ID: " + testId.ToString() + "]  [Type: " + typeStr
                        + "]  [Launch date: " + launched.ToString() + "]\n";

                    result += "[Variants: " + variantsNumber.ToString() + " (including control)]\n";
                     

                    //Displaying ctr and test variant change details
                    //Only "Add to cart" A/B test display supported currently.
                    if(variantsNumber == 2 && type == TestTypesEnum.ADDTOCARTBUTTON)
                    {
                        AddToCartButtonVariation ctrVar = (AddToCartButtonVariation) LiveTests[i].GetVariant(0).GetVariationType();
                        AddToCartButtonVariation tstVar = (AddToCartButtonVariation)LiveTests[i].GetVariant(1).GetVariationType();

                        result += "[Control: ";
                        if (ctrVar.colorPalette == null && ctrVar.addToCartTextCopies == null)
                        {
                            result += "No changes";
                        }
                        else
                        {
                            if (ctrVar.colorPalette != null)
                                result += "Colour: " + ctrVar.colorPalette + "   ";

                            if (ctrVar.addToCartTextCopies != null)
                                result += "Copy change: " + ctrVar.addToCartTextCopies;
                        }
                        result += "]\n";

                        result += "[Test: ";
                        if (tstVar.colorPalette == null && tstVar.addToCartTextCopies == null)
                        {
                            result += "No changes";
                        }
                        else
                        {
                            if (tstVar.colorPalette != null)
                                result += "Colour: " + tstVar.colorPalette + "   ";

                            if (tstVar.addToCartTextCopies != null)
                                result += "Copy change: " + tstVar.addToCartTextCopies;
                        }
                        result += "]\n\n";                        
                    }

                    //Displaying ctr and test variant visitors, cvr and stdev. Works for A/B only.
                    if(TResult != null && variantsNumber == 2)
                    {                        
                        if(TResult.SampleSize.Length > 1 && 
                            TResult.Means.Length > 1 &&
                            TResult.ConvertedSize.Length > 1 &&
                            TResult.STDevs.Length > 1)
                        {       
                                                                 
                            result += "\t[Visitors]\t[Converted]\t[%]\t[Std Dev]\n";

                            result += "CTRL\t" + TResult.SampleSize[0].ToString() + "\t\t"
                                + TResult.ConvertedSize[0].ToString() + "\t\t"
                                + TruncString((TResult.Means[0] * 100).ToString(),4) + "%\t"
                                + TruncString(TResult.STDevs[0].ToString(),5) + "\n";

                            

                            result += "TEST\t" + TResult.SampleSize[1].ToString() + "\t\t"
                                + TResult.ConvertedSize[1].ToString() + "\t\t"
                                + TruncString((TResult.Means[1] * 100).ToString(),4) + "%\t"
                                + TruncString(TResult.STDevs[1].ToString(),5) + "\n\n";

                            result += "[T-score: " + TruncString(TResult.TScores[0].ToString(),6) + "]  "
                                + "[Degree of freedom: " + TResult.DFree[0].ToString()
                                + " (next known is " + TResult.NextDFree[0].ToString() + ")]\n";

                            result += "[Required P for next Degree of Freedom: " + TruncString(TResult.ReqPVal[0].ToString(),6) + "]\n";
                        }
                    }

                    result += "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";                    

                }
            }
            else
            {

                //No live tests found, check if all completed.
                result += "\n No live tests running currently.\n";

                if (AddToCartButton.IsAllVariationTested())
                {
                    result += "All the 'Add To Cart Button' type variants have been tested. The best performing variant is live on 100%."; 
                }

            }

            _CLI.Out(result, false, false);
        }

        private static string TruncString(string input, int maxchar)
        {
            if (input.Length > maxchar)
                return input.Remove(maxchar - 1);
            else
                return input;
        }

        public static void PrintStats(CommandLineInterface _CLI)
        {
            string result = "\n\n********** MVT STATS **********\n\n";


            int TotalMVTs = MVTHistory.GetMVTCount(false);
            int LiveMVTs = MVTHistory.GetMVTCount(true);

            result += "Total MVTs: " + TotalMVTs.ToString() + "\n";
            result += "Live MVTs: " + LiveMVTs.ToString() + "\n";
            
            if(LiveMVTs == 0)
            {
                result += "\n\n********** MVT STATS **********\n\n";
                _CLI.Out(result, false, false);
                return;
            } 

            MultiVariateTest[] LiveTests = MVTHistory.GetLiveTests();          

            if(LiveTests == null || LiveTests.Length == 0)
            {
                result += "\n\n********** MVT STATS **********\n\n";
                
                //No live tests found, check if all completed.
                result += "\n No live tests running currently.\n";
                if (AddToCartButton.IsAllVariationTested())
                {
                    result += "All the 'Add To Cart Button' type variants have been tested. The best performing variant is live on 100%.";
                }

                _CLI.Out(result, false, false);
                return;
            }


            

            for (int i = 0; i < LiveTests.Length; i++)
            {
                result += "\n----\n";

                result += "MVT ID: " + LiveTests[i].Id.ToString() + "\n";
                result += "Variant number (including control): " + LiveTests[i].GetNumberOfVariants().ToString() + "\n";
                result += "Launch date: " + LiveTests[i].GetLaunchDateTime.ToString() + "\n\n";

                MVTResult testResult = LiveTests[i].GetResult;
                if(testResult != null)
                {
                    result += "Variants (including control): " + testResult.Variants.ToString() + "\n";
                    result += "Any variant reached significance: " + testResult.ReachedSignificance.ToString() + "\n";


                    result += "Visitors: ";
                    for(int j = 0; j < testResult.Variants; j++ )
                    {
                        result += "[" + j.ToString() + "]: " + testResult.SampleSize[j] + "\t"; 
                    }
                    result += "\n";

                    result += "Converted visitors: ";
                    for (int j = 0; j < testResult.Variants; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.ConvertedSize[j] + "\t";
                    }
                    result += "\n";

                    result += "Means: ";
                    for (int j = 0; j < testResult.Variants; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.Means[j] + "\t";
                    }
                    result += "\n";

                    result += "Standard Deviations: ";
                    for (int j = 0; j < testResult.Variants; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.STDevs[j] + "\t";
                    }
                    result += "\n\n";

                    result += "T-Score: ";
                    for (int j = 0; j < testResult.Variants-1; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.TScores[j] + "\t";
                    }
                    result += "\n";

                    result += "Degree of Freedom: ";
                    for (int j = 0; j < testResult.Variants - 1; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.DFree[j] + "\t";
                    }
                    result += "\n";

                    result += "Next known Degree Freedom: ";
                    for (int j = 0; j < testResult.Variants - 1; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.NextDFree[j] + "\t";
                    }
                    result += "\n";

                    result += "Required P for next DF: ";
                    for (int j = 0; j < testResult.Variants - 1; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.ReqPVal[j] + "\t";
                    }
                    result += "\n";

                    result += "Is significant: ";
                    for (int j = 0; j < testResult.Variants - 1; j++)
                    {
                        result += "[" + j.ToString() + "]: " + testResult.ReachedSignificance[j] + "\t";
                    }
                    result += "\n";


                    result += "\nNote: [0] is the control variant";

                }
                else
                {
                    result += "Results not analysed yet.\n";
                }

                result += "\n----\n";
            }

            result += "\n\n********** MVT STATS **********\n\n";
            _CLI.Out(result, false, false);
        }
    }
}
