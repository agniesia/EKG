using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG
{
    class ECGoperation
    {
        public static void MinMax(int[] dateECG){
            int[] max=new int[dateECG.Length];
            max[0] = 2;
            max[dateECG.Length - 1] = 2;
            //1 max 0 min
            for (int i = 1; i < dateECG.Length - 1; i++)
            {
                if ((dateECG[i - 1] < dateECG[i]) && (dateECG[i] > dateECG[i + 1]))
                    max[i] = 1;
                if ((dateECG[i - 1] > dateECG[i]) && (dateECG[i] < dateECG[i + 1]))
                    max[i] = 0;
            }


        }
        public static void Direction(int[] dateECG){
            int[] max = new int[dateECG.Length];
            max[0] = 2;
            max[dateECG.Length - 1] = 2;
            //1 rosnie  0 maleje
            List<int> cordinates= new List<int>();
            for (int i = 1; i < dateECG.Length - 1; i++)
            {
                while((dateECG[i - 1] < dateECG[i]))
                    max[i] = 1;
                cordinates.Add(i);
                while((dateECG[i - 1] > dateECG[i]))
                    max[i] = 0;
                cordinates.Add(i);
            }
        }
        private void coordination(int[] dateECG){

        }
        public static void Smoothing(int[] dateECG)
        {
            int[] max = new int[dateECG.Length];
            max[0] = 0;
            max[dateECG.Length - 1] = 0;
            int SizeMask = (int)0.005 * 70;
            int suma=0;
            for(int i=SizeMask;i<dateECG.Length-SizeMask;i++ )
            {
                suma=0;
                for (int k = 0; k < SizeMask; k++)
                    suma += dateECG[i + SizeMask - k];
                max[i]=suma/(SizeMask*2);
            }
        }
        public static void 
    }
}
