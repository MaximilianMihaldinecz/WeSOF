using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof.MVTManager
{
    class MVTAnalyser
    {

        public static MVTResult AnalyseTest(MultiVariateTest test)
        {
            MVTResult result = null;
            int variants = -1;
            int GuidLimitBeforeAbandon;
            int[] SampleSize;
            int[] ConvertedSize;
            double[] Means;
            List<byte>[] Values;
            double[] StdDev;
            double[] Variances;
            double[] TScores;
            int[] DFree;
            int[] NextDFree;
            double[] ReqPVal;
            bool[] ReachedSignificance;


            variants = test.GetNumberOfVariants();
            if (variants < 1)
                return result;

            SampleSize = new int[variants];
            ConvertedSize = new int[variants];
            Means = new double[variants];
            StdDev = new double[variants];
            Values = new List<byte>[variants];
            Variances = new double[variants - 1];
            TScores = new double[variants - 1];
            DFree = new int[variants - 1];
            NextDFree = new int[variants - 1];
            ReqPVal = new double[variants - 1];
            ReachedSignificance = new bool[variants - 1];
            GuidLimitBeforeAbandon = Configuration.STRAT_AVG_DailyTraffic_Expected * Configuration.STRAT_MAX_MVT_RunDaysBeforeAbandon;


            //Getting sample sizes
            for (int i = 0; i < variants; i++)
            {
                SampleSize[i] = test.GetAssignedGuidNumber(i);

                if (Configuration.ANA_SmallSampleSizeOk == false && SampleSize[i] < Configuration.ANA_MinSampleSizePerVariant)
                    return null;

                if (SampleSize[i] == 0)
                    return null;
            }

            //Getting the conversion values as 0 or 1
            for (int i = 0; i < variants; i++)
            {
                Values[i] = test.GetVariantConversionValuesAsNumbers(i);

                if (Values[i] == null)
                    return result;
            }

            //Getting means
            for (int i = 0; i < variants; i++)
            {
                ConvertedSize[i] = 0;
                foreach(byte b in Values[i])
                {
                    ConvertedSize[i] += b;
                }
                Means[i] = (double)ConvertedSize[i] / (double)SampleSize[i];
            }

            //Calculating STD devs
            for (int i = 0; i < variants; i++)
            {

                double sum = 0;

                for(int j = 0; j < SampleSize[i]; j++)
                {
                    double tmp = Values[i][j] - Means[i];
                    tmp *= tmp;
                    tmp /= (SampleSize[i] - 1);

                    sum += tmp;
                }

                StdDev[i] = Math.Sqrt(sum);
            }

            //Calculating variance between sample groups
            //Assuming 0 is control
            for (int i = 1; i < variants; i++)
            {
                double v = StdDev[0] / SampleSize[0];
                v += (StdDev[i] / SampleSize[i]);

                Variances[i - 1] = Math.Sqrt(v);
            }

            //Calculating T-scores
            //Assuming 0 is control
            for (int i = 1; i < variants; i++)
            {
                TScores[i - 1] = Math.Abs(((Means[0] - Means[i]) / Variances[i - 1]));
            }

            //Determine degree of freedom
            for (int i = 1; i < variants; i++)
            {
                DFree[i - 1] = SampleSize[0] + SampleSize[i] - 2;
            }

            //Find the next P value needs to be reached to get significance based on the current freedom
            //Also check if the variants reached significance
            for (int i = 1; i < variants; i++)
            {
                int j;        
                for(j = 0; j < Configuration.ANA_D_Free.Length - 1; j++)
                {
                    if(DFree[i-1] <= Configuration.ANA_D_Free[j])
                    {
                        break;
                    }
                }

                NextDFree[i - 1] = Configuration.ANA_D_Free[j];
                ReqPVal[i - 1] = Configuration.ANA_P_Value[j];

                if(ReqPVal[i -1] < TScores[i - 1])
                {
                    ReachedSignificance[i - 1] = true;
                }
            }

            

            //Setting up results to return
            result = new MVTResult();
            result.Means = Means;
            result.TScores = TScores;
            result.DFree = DFree;
            result.NextDFree = NextDFree;
            result.ReqPVal = ReqPVal;
            result.ReachedSignificance = ReachedSignificance;
            result.IsAnyReachedSignificance = ReachedSignificance.Contains(true);
            result.SampleSize = SampleSize;
            result.ConvertedSize = ConvertedSize;
            result.Variants = variants;
            result.STDevs = StdDev;
            result.GuidLimitBeforeAbandon = GuidLimitBeforeAbandon;

            return result;
        }

    }
}
