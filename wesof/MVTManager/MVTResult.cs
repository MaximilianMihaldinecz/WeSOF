using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof.MVTManager
{
    class MVTResult
    {
        public double[] Means;
        public double[] TScores;
        public double[] STDevs;
        public int[] DFree;
        public int[] NextDFree;
        public double[] ReqPVal;
        public bool[] ReachedSignificance;
        public int[] SampleSize;
        public int[] ConvertedSize;
        public int GuidLimitBeforeAbandon;

        public int Variants;
        public bool IsAnyReachedSignificance;
    }
}
