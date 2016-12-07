using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof.MVTManager
{
    static class ConversionManager
    {


        static List<ConversionTrigger> _Triggers = new List<ConversionTrigger>();


        public static void AddConversionTriggerIfnotExists(ConversionTrigger trigger)
        {
            for (int i = 0; i < _Triggers.Count; i++)
            {
                if(_Triggers[i].Href == trigger.Href)
                {
                    return;
                }
            }

            _Triggers.Add(trigger);
        }

        internal static void CheckUrlAgainstConversionTriggers(string requestedUrl, string guid)
        {
            if (requestedUrl == null || guid == null)
                return;

            for (int i = 0; i < _Triggers.Count; i++)
            {
                if (_Triggers[i].Href == requestedUrl)
                {
                    TriggerTrigged(guid);
                    break;
                }
            }
        }

        private static void TriggerTrigged(string guid)
        {
            MVTHistory.IncrementConversionOnGuid(guid);                                               
        }

    }
}
