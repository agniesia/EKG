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
using System.Threading;

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
            enablefalse();
            
        }
        
        int[] ekgdata;
        int[] mark;
        
        private async void open_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker FOP = new FileOpenPicker();

            FOP.ViewMode = PickerViewMode.List;

            FOP.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            FOP.FileTypeFilter.Add(".ascii");
            FOP.FileTypeFilter.Add(".txt");

            StorageFile filepom = await FOP.PickSingleFileAsync();

            if (filepom != null)
            {

                string fileContent = await FileIO.ReadTextAsync(filepom);

                string[] dane = (fileContent.Split(' '));
                if (dane[dane.Length-1]=="")
                    dane[dane.Length-1]="0";
                ekgdata = dane.AsParallel().Select(x => int.Parse(x, NumberStyles.Integer)).ToArray();
                //System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(50));
                mark = new int[ekgdata.Length];
                plot.Children.Clear();
                
               
                ploting(normalizedata(ekgdata),(int)plot.ActualWidth,mark);

                enabletrue();
                moove.Value = 0;
                

            }
        }
        private  void waitcursor(){
             Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);
             

        }
        private void nowaitcursor()
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            
        }
        private void enablefalse()
        {
            moove.IsEnabled = false;
            speddslider.IsEnabled = false;
            pley.IsEnabled = false;
            pause.IsEnabled = false;
            operations.IsEnabled = false;
        }
        private void enabletrue()
        {
            moove.IsEnabled = true;

            speddslider.IsEnabled = true;
            pley.IsEnabled = true;
            
            operations.IsEnabled = true;
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
            myPolygon.StrokeThickness = 4;
            myPolygon.HorizontalAlignment = HorizontalAlignment.Left;
            myPolygon.VerticalAlignment = VerticalAlignment.Center;
            //************************************************************//
            
            

            PointCollection points = new PointCollection();
            
           
                for (int i = (max - (int)plot.ActualWidth), j = 0; i < max; i++, j++)
                {
                    draw(maska, i, j);
                    points.Add(new Point(j, plot.ActualHeight - ekgdata[i]));
                    
                    
                }
            

            myPolygon.Points = points;

            plot.Children.Add(myPolygon);
            
            
        }
        private void draw(int[] maska ,int i, int j)
        {
            try
            {
                if (maska[i] == 1)
                {

                    //Rectangle rect = new Rectangle();
                    //rect.Width = 1;
                    //rect.Height = 10;
                    //rect.Stroke = new SolidColorBrush(Colors.LightPink);
                    //rect.Fill = new SolidColorBrush(Colors.LightPink);

                    //Canvas.SetLeft(rect, j);
                    //Canvas.SetTop(rect, plot.ActualHeight - ekgdata[i]);
                    Line myLine = new Line();
                    myLine.Stroke = new SolidColorBrush(Colors.Aqua);
                    myLine.StrokeThickness = 1;
                    myLine.X1 = j;
                    myLine.X2 = j;
                    myLine.Y1 = plot.ActualHeight - ekgdata[i];
                    myLine.Y2 = plot.ActualHeight - ekgdata[i] + 10;

                    plot.Children.Add(myLine);
                }
                //else if (maska[i] == 0)
                //{
                //    //Rectangle rect = new Rectangle();
                //    //rect.Width = 10;
                //    //rect.Height = 10;
                //    //rect.Stroke = new SolidColorBrush(Colors.MediumSeaGreen);
                //    //rect.Fill = new SolidColorBrush(Colors.MediumSeaGreen);
                //    //rect.StrokeThickness = 2;
                //    //Canvas.SetLeft(rect, j);
                //    //Canvas.SetTop(rect, plot.ActualHeight - ekgdata[i]);
                //    //Line myLine = new Line();
                //    //myLine.Stroke = new SolidColorBrush(Colors.Brown);
                //    //myLine.StrokeThickness = 50;
                //    //myLine.X1 = j;//j + 1;
                //    //myLine.X2 = j;// j + 1;
                //    //myLine.Y1 = plot.ActualHeight - ekgdata[i];
                //    //myLine.Y2 = plot.ActualHeight - ekgdata[i] + 1;

                //    //plot.Children.Add(myLine);
                //    //plot.Children.Add(rect);

                //}
            }
            catch (Exception t)
            {
                
            }
           

        }
        

        private void moove_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            

        }

        private void moove_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            
            int pozycja =(int) (moove.Value * ekgdata.Length / 10000);
            if (pozycja < (int)plot.ActualWidth) pozycja = (int)plot.ActualWidth;
            plot.Children.Clear();
            ploting(ekgdata, pozycja,mark);
        }


        private async System.Threading.Tasks.Task  speedy_show(int n)
        {

            int poz = (int)(moove.Value);
            int pozycja = 0;
            for (int i = poz; i < moove.Maximum; i+=1)
            {

                n = (int)speddslider.Value;
                pozycja = (int)(i * ekgdata.Length / 10000);
                if (pozycja < (int)plot.ActualWidth) pozycja = (int)plot.ActualWidth;
                plot.Children.Clear();
                if (shouldStop == true || i == moove.Maximum-1)
                {
                    ploting(ekgdata, pozycja, mark);
                    shouldStop = false;
                    break;
                }
                ploting(ekgdata, pozycja, mark);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(n));
                
            }
            
        }
        private volatile bool shouldStop = false;
        
        private async  void pley_Click(object sender, RoutedEventArgs e)
        {
            
            moove.IsEnabled = false;
            pley.IsEnabled = false;
            pause.IsEnabled = true;
            int n = (int)(10 * speddslider.Value);

            await speedy_show(n);


            pley.IsEnabled = true;
            moove.IsEnabled = true;
            moove.Value = 0;
            
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            
            shouldStop = true;
            pause.IsEnabled = false;
        }

        private void beforaction()
        {
            waitcursor();
            apptop.IsEnabled = false;
        }
        private void afteraction()
        {
            var pozycja = (int)(moove.Value * ekgdata.Length / 10000);
            if (pozycja < (int)plot.ActualWidth) pozycja = (int)plot.ActualWidth;
            
            plot.Children.Clear();
            ploting(ekgdata, (int)pozycja, mark);

            operatiofly1.Hide();
            app.IsOpen = false;
            apptop.IsEnabled = true;
            nowaitcursor();
        }

        private void smoothbutton_Click(object sender, RoutedEventArgs e)
        {
            beforaction();
            mark = new int[ekgdata.Length];
            ekgdata = ECGoperation.Smoothing(ekgdata);
            afteraction();
        }

        private void direction_Click(object sender, RoutedEventArgs e)
        {
            beforaction();
            mark = ECGoperation.Direction(ekgdata);
            afteraction();
        
        }

        private void small_interwals_Click(object sender, RoutedEventArgs e)
        {
            beforaction();
            mark = ECGoperation.todraw(ekgdata);
            afteraction();
        }

        private void after_merge_Click(object sender, RoutedEventArgs e)
        {
            beforaction();
            mark = ECGoperation.scalanie(ekgdata);
            afteraction();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            beforaction();
            mark = ECGoperation.points(ekgdata);
            afteraction();
        }

        private void maxminbutton_Click(object sender, RoutedEventArgs e)
        {
            beforaction();
            mark = ECGoperation.MinMax(ekgdata);
            afteraction();
        }

        private void ekstrapionts_Click(object sender, RoutedEventArgs e)
        {
            beforaction();
            mark = ECGoperation.pointimportant(ekgdata);
            afteraction();
        }

       

        
    }
    
}
