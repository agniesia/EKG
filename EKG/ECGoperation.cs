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
            max[0] = 0;
            max[dateECG.Length - 1] = 0;
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
            max[0] = 0;
            
            //1 rosnie  0 maleje
            //List<int> cordinates= new List<int>();
            

            for (int i = 1; i < dateECG.Length ; i++)
            {
                if((dateECG[i - 1] < dateECG[i])){
                    max[i] = 1;
                    
                }
                else if((dateECG[i - 1] > dateECG[i]))
                    max[i] = 0;
                
            }
            return max;
        }
        private static List<int> coordination(int[] direction)
        {
            List<int> cordinates = new List<int>();
            cordinates.Add(0);

            for (int i = 0; i < direction.Length - 1; i++)
            {
                if (direction[i] != direction[i + 1])
                    cordinates.Add(i+1);
            }
            if (!(cordinates.Contains(direction.Length - 1) == true))
                cordinates.Add(direction.Length - 1);
            return cordinates;
            
        }
        public static int[] Smoothing(int[] dateECG)
        {
            int SizeMask = 5;
            int[] max = new int[dateECG.Length + 2 * SizeMask];
            max[0] = 0;
            max[dateECG.Length - 1] = 0;
           
            int suma=0;
            int t = 5;
            List<int> tempecg = dateECG.ToList();
            
             for (int i = 0; i < SizeMask; i++)
                 tempecg.Add(0);
             tempecg.Reverse();
             for (int i = 0; i < SizeMask; i++)
                 tempecg.Add(0);
             tempecg.Reverse();
             dateECG= tempecg.ToArray();
            while (t > 0)
            {
                for (int i = SizeMask; i < dateECG.Length - SizeMask; i++)
                {
                    suma = 0;
                    for (int k = 0; k < 2*SizeMask; k++)
                        suma += dateECG[i + SizeMask - k];
                    max[i] = suma / (SizeMask * 2);
                }
                dateECG = (int[])(max.Clone());
                
                t--;
            }
            return dateECG;
        }
        public static List<Tuple<int, int>> MarkToSacaling(int[] dateECG, int[] direction)
        {
            List<int> cordinates = coordination(direction);
            List<Tuple<int, int>> scaling = new List<Tuple<int, int>>();
            // zmiennosc obszaru
            int ECGdiference =(int)(0.01 * (dateECG.Max() - dateECG.Min()));
            // dlugosc obszaru
            int min_long=(int)(15);

            //dodaje odscinki o wsp poczz i kon w jednym tuplu . mozliwe graniczne wsp 0-x i y-kon;
            for (int i = 0; i < cordinates.Count-1; i++)
            {
                if(cordinates.ElementAt(i+1)-cordinates.ElementAt(i)<min_long)
                {
                   
                    scaling.Add(new Tuple<int, int>(cordinates.ElementAt(i), cordinates.ElementAt(i + 1)));
                   
                    
                }
                else if (Math.Abs(dateECG[cordinates.ElementAt(i)]-dateECG[cordinates.ElementAt(i+1)])<ECGdiference)
                {
                    scaling.Add(new Tuple<int, int>(cordinates.ElementAt(i), cordinates.ElementAt(i + 1)));

                }

            }
            return scaling;
        }
        public static int[]  todraw(int[] dateECG){

            List<Tuple<int, int>> c = MarkToSacaling(dateECG, Direction(dateECG));
            
            int[] mark = new int[dateECG.Length];


            for (int i = 0; i < c.Count; i++)
            {
                for (int k = c.ElementAt(i).Item1; k < c.ElementAt(i).Item2; k++)
                    mark[k] = 1;
            }
            return mark;
        }

        public static int[] scalanie(int[] dateECG){
            
            var directions= Direction(dateECG);
            var coordinates = coordination(directions);
            List<Tuple<int, int>> toscale= MarkToSacaling(dateECG, directions);
            //var coordinates=coordination(directions);
            var tofill = 0;
            if(toscale.First().Item1==0){
                 tofill=directions[toscale.ElementAt(1).Item1];
                for (int k = 0; k < toscale.First().Item2; k++)
                    directions[k] =tofill ;
                toscale.RemoveAt(0);
            }
            var s = toscale.Last().Item2;
            if (toscale.Last().Item2 == (dateECG.Length-1))
            {
                tofill = directions[toscale.ElementAt(toscale.Count-2).Item1];
                for (int k = toscale.Last().Item1; k < toscale.Last().Item2; k++)
                    directions[k] = tofill;
                toscale.RemoveAt(toscale.Count - 1);
            }

            var toscaling = toscale.ToList();
            
            
                for (int i = toscaling.Count - 1; i > 0; i--)
                {

                    if (directions[toscaling.ElementAt(i).Item1-1] == directions[toscaling.ElementAt(i).Item2])
                        
                    {
                        //check what direction have to fill 
                        
                        tofill = directions[toscaling.ElementAt(i).Item1-1];
                        //fill small elelement
                        for (int k = toscaling.ElementAt(i).Item1; k < toscaling.ElementAt(i).Item2; k++)
                            directions[k] = tofill;
                        toscale.RemoveAt(i);


                    }
                }

                
                toscaling = toscale.ToList();
            

            for (int i = 0; i < toscaling.Count; i++)
            {
                //koncowy indeks 1 obszaru
                var index1last = coordinates.IndexOf(toscaling.ElementAt(i).Item1);
                var index1first = index1last - 1;
                //pierwszy indeks drugiego obszaru
                var index2first = coordinates.IndexOf(toscaling.ElementAt(i).Item2);
                var index2last = index2first + 1;

                var speedy1 = Math.Abs(dateECG[coordinates[index1first]] - dateECG[coordinates[index1last]]);
                var speedy2 = Math.Abs(dateECG[coordinates[index2first]] - dateECG[coordinates[index2last]]);
                if (speedy1 < speedy2)
                {
                    tofill = directions[toscaling.ElementAt(i).Item1 - 1];
                    for (int k = toscaling.ElementAt(i).Item1; k < toscaling.ElementAt(i).Item2; k++)
                        directions[k] = tofill;
                }
                else
                {
                   tofill = directions[toscaling.ElementAt(i).Item2 + 1];
                    for (int k = toscaling.ElementAt(i).Item1; k < toscaling.ElementAt(i).Item2; k++)
                        directions[k] = tofill;
                }

            }
            return directions;
        }
        public static int[] points(int[] dateECG)
        {
            var markowanie=scalanie(dateECG);
            int[] mark = new int[dateECG.Length];
            var cordinates = coordination(markowanie);
            List<Tuple<int, int>> interval = new List<Tuple<int, int>>();
            for (int i = 0; i < cordinates.Count - 1; i++)
            {
               interval.Add(new Tuple<int, int>(cordinates.ElementAt(i), cordinates.ElementAt(i + 1)));
            }
            var suma = 0;
            int t=0;
            
            double sr=0;
            for (int i = 0; i < interval.Count; i++)
            {
                suma = 0;
                t=0;
                for (int k = interval.ElementAt(i).Item1; k < interval.ElementAt(i).Item2-1; k++)
                {
                    suma += Math.Abs(dateECG[k]-dateECG[k+1]);
                    t++;
                }
                sr = 0.3*(suma / t);

                for (int k = interval.ElementAt(i).Item1+1; k < interval.ElementAt(i).Item2-1; k++)
                {
                    
                    if (Math.Abs(dateECG[k]-dateECG[k-1]) < sr && Math.Abs(dateECG[k]-dateECG[k+1])<sr)
                        mark[k] = 1;
                }

            }
            return mark;
           
        }
        public static int[] pointimportant(int[] dateECG)
        {
            var min=dateECG.Min();

            var max=dateECG.Max();
           // var mark1 = points(dateECG);
            //var mark2 = MinMax(dateECG);
            var mark3= new int[dateECG.Length];
            for(int i=5; i<mark3.Length-5;i++){
                    if(dateECG[i]<(int)(min+50))
                        mark3[i]=1;

                }
            return mark3;
        }
    }
}
