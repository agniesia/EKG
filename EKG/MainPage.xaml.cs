using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EKG
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            moove.IsEnabled = false;
        }
        int[] ekgdata;
        
        private async void open_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker FOP = new FileOpenPicker();

            FOP.ViewMode = PickerViewMode.List;

            FOP.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            FOP.FileTypeFilter.Add(".ascii");

            StorageFile filepom = await FOP.PickSingleFileAsync();

            if (filepom != null)
            {

                string fileContent = await FileIO.ReadTextAsync(filepom);

                string[] dane = (fileContent.Split(' '));
                if (dane[dane.Length-1]=="")
                    dane[dane.Length-1]="0";
                ekgdata = dane.AsParallel().Select(x => int.Parse(x, NumberStyles.Integer)).ToArray();
                //System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(50));
                ekgdata = ECGoperation.Smoothing(ekgdata);
                var mark = ECGoperation.Direction(ekgdata);
                ploting(normalizedata(ekgdata), (int)plot.ActualWidth,mark);
                moove.IsEnabled = true;
                moove.Value = 0;
                

            }
        }

        private int[] normalizedata(int[] data)
        {   var max= data.Max();
            var min = data.Min();
            double[] nowy = new double[data.Length];
            for (int i = 0; i < data.Length; i += 1)
            {
                data[i] =(int )((((data[i]) - min) / Convert.ToDouble(max - min)) * plot.ActualHeight);
            }
            //data[i]=data[].AsParallel().Select(x => x =((x - min )/ (max-min))*a).ToArray();
            
            return data;
        }
        private void  ploting(int[] data ,int max,int[] maska=null){
            
            Polyline myPolygon = new Polyline();
            myPolygon.Stroke = new SolidColorBrush(Colors.Black);

            //myPolygon.Opacity = 0.5;
            myPolygon.StrokeThickness = 2;
            myPolygon.HorizontalAlignment = HorizontalAlignment.Left;
            myPolygon.VerticalAlignment = VerticalAlignment.Center;
            //************************************************************//
            
            

            PointCollection points = new PointCollection();
            
           
                for (int i = (max - (int)plot.ActualWidth), j = 0; i < max; i++, j++)
                {
                    draw(maska, j, (int)plot.ActualHeight - ekgdata[i]);
                    points.Add(new Point(j, plot.ActualHeight - ekgdata[i]));

                    //draw(maska, j,i );
                    
                }
            

            myPolygon.Points = points;

            plot.Children.Add(myPolygon);
            
            
        }
        private void draw(int[] maska ,int j, int i)
        {
            if(maska[j]==1){

                
                Line myLine = new Line();
                myLine.Stroke = new SolidColorBrush(Colors.Aqua);
                myLine.StrokeThickness = 4;
                myLine.X1 = j + 1;
                myLine.X2 = j + 1;
                myLine.Y1 = i;// plot.ActualHeight - ekgdata[i];
                myLine.Y2 = i + 1;//plot.ActualHeight - ekgdata[i] + 1;

                plot.Children.Add(myLine);
            }
            else if (maska[j] == 0)
            {
                
                Line myLine = new Line();
                myLine.Stroke = new SolidColorBrush(Colors.Black);
                myLine.StrokeThickness = 4;
                myLine.X1 = i;//j + 1;
                myLine.X2 = i;// j + 1;
                myLine.Y1 = j+1;// ; plot.ActualHeight - ekgdata[i];
                myLine.Y2 = j + 1;// plot.ActualHeight - ekgdata[i] + 1;

                plot.Children.Add(myLine);

            }


        }
        

        private void moove_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            

        }

        private void moove_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var mark = ECGoperation.Direction(ekgdata);
            int pozycja =(int) (moove.Value * ekgdata.Length / 10000);
            if (pozycja < (int)plot.ActualWidth) pozycja = (int)plot.ActualWidth;
            plot.Children.Clear();
            ploting(ekgdata, pozycja,mark);
        }

        
        
    }
    
}
