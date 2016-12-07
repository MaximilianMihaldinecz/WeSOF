using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wesof.MVTModules;
using wesof.MVTModules.AddToCartButton;

namespace wesof.MVTManager
{
    static class MVTHistory
    {
        static private List<MultiVariateTest> _History = new List<MultiVariateTest>();
        static private CommandLineInterface _CLI;

        internal static int GetMVTCount(bool liveTestsOnly)
        {
            if (liveTestsOnly == false)
                return _History.Count;


            int result = 0;            
            for(int i = 0; i<_History.Count; i++)
            {
                if (_History[i].IsActive)
                    result++;
            }
            return result;
        }

        internal static int GetMVTCount(TestResultType type)
        {            
            
            int result = 0;
            for (int i = 0; i < _History.Count; i++)
            {
                if (_History[i].GetResultType() == type)
                    result++;
            }
            return result;
        }

        public static MultiVariateTest[] GetAllTests()
        {
            return _History.ToArray();
        }


        public static void SetCLI(ref CommandLineInterface cli)
        {
            _CLI = cli;
        }

        public static MultiVariateTest[] GetLiveTests()
        {
            List<MultiVariateTest> result = new List<MultiVariateTest>();
            for (int i = 0; i < _History.Count; i++)
            {
                if (_History[i].IsActive)
                    result.Add(_History[i]);
            }

            if (result.Count == 0)
                return null;

            return result.ToArray();
        }


        public static void UnAsssignGUIDfromFinishedTests(ref ServerResponse sRep)
        {
            if (sRep.Cookie.ModifiedMVTAssignments == null)
                return;

            for(int i = 0; i< sRep.Cookie.ModifiedMVTAssignments.Length; i++)
            {
                string s = sRep.Cookie.ModifiedMVTAssignments[i];

                string[] tmp = s.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                int index = IndexById(int.Parse(tmp[0]));

                //check if this is a valid mvt id
                if (index == -1)
                {
                    sRep.Cookie.ModifiedMVTAssignments[i] = String.Empty;
                    if (Configuration.IsDebugModeOn)
                        _CLI.Out("Cannot find the MVT id sent by the client.", true, false);

                    continue;
                }

                //Check if the MVT is still live                
                if (_History[index].IsActive == false)
                {
                    sRep.Cookie.ModifiedMVTAssignments[i] = String.Empty;                    
                }
            }

            sRep.Cookie.CleanUpModifiedMVTList();
        }

        internal static void IncrementConversionOnGuid(string guid)
        {
            
            for(int i=0; i<_History.Count; i++)
            {
                if(_History[i].IsActive)
                {
                    Variation v = _History[i].GetVariantWithGuid(guid);
                    if(v!=null)
                    {
                        bool previousConversionState = v.ConvertGuid(guid);

                        if(previousConversionState == false)
                        {
                            UpdateTestAnalysis(_History[i]);
                        }
                    }
                }
            }

            _CLI.RefreshStatDashboard();
        }

        private static void UpdateTestAnalysis(MultiVariateTest multiVariateTest)
        {
            multiVariateTest.UpdateTestResult();
        }

        public static Variation[] MatchLiveMVTWithTestType(string[] mvtlist, TestTypesEnum testType)
        {
            if (mvtlist == null)
                return null;

            List<Variation> result = new List<Variation>();
            foreach(string s in mvtlist)
            {
                string[] tmp = s.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                int iMvt = int.Parse(tmp[0]);
                int iVar = int.Parse(tmp[1]);

                int index = IndexById(iMvt);

                //check if this is a valid mvt id
                if(index == -1)
                {
                    if (Configuration.IsDebugModeOn)
                        _CLI.Out("Cannot find the MVT id sent by the client.", true, false);
                    continue;
                }

                //Check if the assigned mvt is relevant
                if(_History[index].IsActive == false)
                {
                    if (Configuration.IsDebugModeOn)
                        _CLI.Out("Error. The MVT sent by the client is not active, but has not been removed during the cleaning.", true, false);
                    continue;
                }

                //Check if the type is appropriate
                if (_History[index].TestType == testType)
                {
                    //Get the containing variant
                    Variation v = _History[index].GetVariant(iVar);
                    
                    if(v == null)
                    {
                        if (Configuration.IsDebugModeOn)
                            _CLI.Out("Error. Found mvt, but could not get variation index.", true, false);
                        continue;
                    }
                    else
                    {
                        result.Add(v);
                    }
                }
            }

            if (result.Count == 0)
                return null;
            else
                return result.ToArray();
        }

        internal static Variation GetVariant(int MVTId, int varId)
        {
            int mvtindex = IndexById(MVTId);
            if(mvtindex > -1)
            {
                return _History[mvtindex].GetVariant(varId);
            }

            return null;
        }

        static private int IndexById(int id)
        {
            int result;
            for(result = 0; result<_History.Count; result++ )
            {
                if (_History[result].Id == id)
                    return result;
            }
            return -1;
        }


        /// <summary>
        /// Creates a new live test with the given type, and defines a control.
        /// </summary>
        /// <param name="testType"></param>
        /// <returns></returns>
        public static int CreateNewLiveMVT(TestTypesEnum testType)
        {
            int id = _History.Count;
            MultiVariateTest MVT = new MultiVariateTest(id, testType);
            _History.Add(MVT);
            return id;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="testType"></param>
        /// <returns>With a suitable live MVT id for the given type of test</returns>
        public static int GetSuitableLiveMVTforNewGUIDAssignment(TestTypesEnum testType)
        {
            int i;
            for(i = 0; i<_History.Count; i++)
            {
                if (_History[i].IsActive && _History[i].TestType == testType)
                    return i;
            }           
            
            return -1;
        }

        internal static void AddNewTestVariantToMVT(int mvtid, ref VariationType testVariant)
        {
            int i;
            for (i = 0; i < _History.Count; i++)
            {
                if (_History[i].Id == mvtid)
                    break;
            }

            if(i<_History.Count)
            {
                _History[i].AddTestVariant(ref testVariant);
            }
        }

        internal static int AssignGuidToMVTEvenly(int mvtId, string guid)
        {
            int i;
            for (i = 0; i < _History.Count; i++)
            {
                if (_History[i].Id == mvtId)
                    break;
            }

            if (i < _History.Count)
            {
                return _History[i].AssignGuidEvenly(guid);
            }

            return -1;
        }
    }
}
