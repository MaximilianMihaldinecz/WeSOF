using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace wesof
{
    class Configuration
    {
        private static Int32 listenerPortNumber = 12345;
        private static IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        private static IPAddress webServerAddress = IPAddress.Parse("192.168.0.14");
        private static Int32 webServerPortNumber = 80;
        private static int readingBufferLength = 1024;
        private static bool isDebugModeOn = true;
        private static bool isDisplayRawTraffic = false;
        private static bool isDisplayConnections = false;
        private static bool isKeepAliveAlowed = false;
        private static bool isDashboardOn = false;
        private static int clientThreadRefreshInterval = 40;
        private static int closeConnectionAfterTime = 100;
        private static bool proxyHtmlFilesOnly = true;
        private static bool isMVTEnabled = true;
        private static bool isAnalysisRunWithNewGuid = true;
        private static string weSOFHtmlTagStartString = "WESOF_START";
        private static string weSOFHtmlTagStopString = "WESOF_STOP";
        private static string wesofCookieGuidName = "wesof_guid";
        private static string wesofCookieMVTAssignmentsName = "wesof_mvtassignments";

        public const string TAG_ISTESTABLE = "TESTINGENABLED";
        public const string TAG_OBJECTGROUPID = "OBJECTGROUPID";
        public const string TAG_OBJECTTYPE = "OBJECTTYPE";


        public static string[] MVT_MODULES_LIST = { "ADDTOCARTBUTTON" };
        public static TestTypesEnum[] MVT_MODULES_LIST_ENUMS = { TestTypesEnum.ADDTOCARTBUTTON };

        //START Analysis settings
        public static int[] ANA_D_Free = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 60, 80, 100, 1000, 10000 }; // Degree of freedoms for the matchin t-score values
        public static double[] ANA_P_Value = 
            { 12.71, 4.303, 3.182, 2.776, 2.571, 2.447, 2.365, 2.306, 2.262, 2.228, 2.086, 2.042, 2.021, 2.0, 1.990, 1,984, 1.962, 1.960}; //95% significance P values to the related degree of freedoms

        public static int ANA_MinSampleSizePerVariant = 5;
        public static bool ANA_SmallSampleSizeOk = true;
        //END Analysis settings

        //Test strategy controll START
        public static int STRAT_AVG_DailyTraffic_Expected = 2; //Expected daily visitors. This is used if no analytics data available... e.g. Demo
        public static int STRAT_MAX_MVT_RunDaysBeforeAbandon = 60; //Max days to run a test before consider it inconclusive.
        //Test strategy controll END


        private static Controller mainController;


        public static int ListenerPortNumber
        {
            get
            {
                return listenerPortNumber;
            }

            set
            {
                listenerPortNumber = value;
            }
        }

        public static IPAddress LocalAddress
        {
            get
            {
                return localAddress;
            }

            set
            {
                localAddress = value;
            }
        }

        public static int ReadingBufferLength
        {
            get
            {
                return readingBufferLength;
            }

            set
            {
                readingBufferLength = value;
            }
        }

        public static bool IsDebugModeOn
        {
            get
            {
                return isDebugModeOn;
            }

            set
            {
                isDebugModeOn = value;
            }
        }

        public static bool IsDisplayRawTraffic
        {
            get
            {
                return isDisplayRawTraffic;
            }

            set
            {
                isDisplayRawTraffic = value;
            }
        }

        public static IPAddress WebServerAddress
        {
            get
            {
                return webServerAddress;
            }

            set
            {
                webServerAddress = value;
            }
        }

        public static int WebServerPortNumber
        {
            get
            {
                return webServerPortNumber;
            }

            set
            {
                webServerPortNumber = value;
            }
        }

        public static bool IsKeepAliveAlowed
        {
            get
            {
                return isKeepAliveAlowed;
            }

            set
            {
                isKeepAliveAlowed = value;
            }
        }

        public static int ClientThreadRefreshInterval
        {
            get
            {
                return clientThreadRefreshInterval;
            }

            set
            {
                clientThreadRefreshInterval = value;
            }
        }

        public static int CloseConnectionAfterTime
        {
            get
            {
                return closeConnectionAfterTime;
            }

            set
            {
                closeConnectionAfterTime = value;
            }
        }

        public static bool ProxyHtmlFilesOnly
        {
            get
            {
                return proxyHtmlFilesOnly;
            }

            set
            {
                proxyHtmlFilesOnly = value;
            }
        }

        public static bool IsMVTEnabled
        {
            get
            {
                return isMVTEnabled;
            }

            set
            {
                isMVTEnabled = value;
            }
        }

        public static string WeSOFHtmlTagStartString
        {
            get
            {
                return weSOFHtmlTagStartString;
            }

            set
            {
                weSOFHtmlTagStartString = value;
            }
        }

        public static string WeSOFHtmlTagStopString
        {
            get
            {
                return weSOFHtmlTagStopString;
            }

            set
            {
                weSOFHtmlTagStopString = value;
            }
        }

        internal static Controller MainController
        {
            get
            {
                return mainController;
            }

            set
            {
                mainController = value;
            }
        }

        public static string WesofCookieGuidName
        {
            get
            {
                return wesofCookieGuidName;
            }

            set
            {
                wesofCookieGuidName = value;
            }
        }

        public static string WesofCookieMVTAssignmentsName
        {
            get
            {
                return wesofCookieMVTAssignmentsName;
            }

            set
            {
                wesofCookieMVTAssignmentsName = value;
            }
        }

        public static bool IsDisplayConnections
        {
            get
            {
                return isDisplayConnections;
            }

            set
            {
                isDisplayConnections = value;
            }
        }

        public static bool IsDashboardOn
        {
            get
            {
                return isDashboardOn;
            }

            set
            {
                isDashboardOn = value;
            }
        }

        public static bool IsAnalysisRunWithNewGuid
        {
            get
            {
                return isAnalysisRunWithNewGuid;
            }

            set
            {
                isAnalysisRunWithNewGuid = value;
            }
        }
    }
}
