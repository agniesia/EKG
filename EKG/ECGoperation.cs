using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG
{
    class ECGoperation
    {
        public static int[] MinMax(int[] dateECG){
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
            return max;

        }
        public static int[] Direction(int[] dateECG){
            int[] max = new int[dateECG.Length];
            max[0] = 2;
            max[dateECG.Length - 1] = 2;
            //1 rosnie  0 maleje
            //List<int> cordinates= new List<int>();
            

            for (int i = 1; i < dateECG.Length - 1; i++)
            {
                if((dateECG[i - 1] < dateECG[i])){
                    max[i] = 1;
                    
                }
                else if((dateECG[i - 1] >= dateECG[i]))
                    max[i] = 0;
                
            }
            return max;
        }
        private static List<int> coordination(int[] dateECG)
        {
            List<int> cordinates = new List<int>();
            cordinates.Add(0);
            
            for (int i = 0; i < dateECG.Length - 1; i++){
                if(dateECG[i]!=dateECG[i+1])
                    cordinates.Add(i+1);
            }
            return cordinates;
            
        }
        public static int[] Smoothing(int[] dateECG)
        {
            int[] max = new int[dateECG.Length];
            max[0] = 0;
            max[dateECG.Length - 1] = 0;
            int SizeMask = 5;// (int)(0.05 * 70);
            int suma=0;
            int t = 5;
            while (t > 0)
            {
                for (int i = SizeMask; i < dateECG.Length - SizeMask; i++)
                {
                    suma = 0;
                    for (int k = 0; k < SizeMask; k++)
                        suma += dateECG[i + SizeMask - k];
                    max[i] = suma / (SizeMask * 2);
                }
                dateECG=(int[])(max.Clone());
                
                t--;
            }
            return dateECG;
        }
        public static List<int> MarkToSacaling(int[] dateECG)
        {
            List<int> cordinates = coordination(dateECG);
            int ECGdiference = (int)(0.01 * (dateECG.Max() - dateECG.Min()));
            List<int> scaling = new List<int>();
            int min_long=(int)0.03*70;
            for (int i = 0; i < cordinates.Capacity-1; i++)
            {
                if(cordinates.ElementAt(i)-cordinates.ElementAt(i+1)<min_long)
                {
                    scaling.Add(cordinates.ElementAt(i));
                    scaling.Add(cordinates.ElementAt(i+1));
                    
                }
                else if (Math.Abs(dateECG[cordinates.ElementAt(i)]-dateECG[cordinates.ElementAt(i+1)])<ECGdiference)
                {
                    scaling.Add(cordinates.ElementAt(i));
                    scaling.Add(cordinates.ElementAt(i + 1));

                }

            }
            return scaling;
        }
        public static int[] scalanie(int[] dateECG){
            List<int> toscaling=MarkToSacaling(dateECG);
            var directions= Direction(dateECG);
            var coordinates=coordination(directions);
            for (int i = 0; i < toscaling.Capacity; i++)
            {
                if((directions[toscaling.ElementAt(i)-1]==directions[toscaling.ElementAt(i+1)+1])&&
                    (directions[toscaling.ElementAt(i)-1]==directions[toscaling.ElementAt(i)]))
                {
                    var tofill = directions[toscaling.ElementAt(i) - 1];
                    for (int k = toscaling.ElementAt(i); k < toscaling.ElementAt(i + 1); k++)
                        directions[k] = tofill;
                    toscaling.RemoveAt(i);
                    toscaling.RemoveAt(i + 1);
                    
                }
            }
            for (int i = 0; i < toscaling.Capacity; i++)
            {
                //koncowy indeks 1 obszaru
                var index1last = coordinates.IndexOf(toscaling.ElementAt(i));
                var index1first=index1last-1;
                //pierwszy indeks drugiego obszaru
                var index2first = coordinates.IndexOf(toscaling.ElementAt(i + 1));
                var index2last = index2first + 1;
                var speedy1 = Math.Abs(dateECG[index1first] - dateECG[index1last]);
                var speedy2 = Math.Abs(dateECG[index2first] - dateECG[index2last]);
                if (speedy1 < speedy2)
                {
                    var tofill = directions[toscaling.ElementAt(i) - 1];
                    for (int k = toscaling.ElementAt(i); k < toscaling.ElementAt(i + 1); k++)
                        directions[k] = tofill;
                }
                else
                {
                    var tofill = directions[toscaling.ElementAt(i+1)+1];
                    for (int k = toscaling.ElementAt(i); k < toscaling.ElementAt(i + 1); k++)
                        directions[k] = tofill;
                }

            }
            return directions;
        }
        public static void points(int[] dateECG)
        {
            var coordinations = coordination(dateECG);
            foreach (int x in coordinations)
            {
                
            }
        }
    }
}
