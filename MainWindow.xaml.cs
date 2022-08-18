using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using ScottPlot;
using MathNet;
using System.Windows.Markup;


namespace Raman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Tuple<List<string>, List<string>, List<string>> data; //Holds data returned from ReadCsv()

        private List<string> pixelString; //For The List Box
        private List<string> RamanShiftString; //For The List Box
        private List<string> yString; //For The List Box

        private double[] x_coords; //For the graph
        private double[] y_coords; //For the graph

        private void TestCsvOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); //Open File viewer
            Nullable<bool> result = openFileDialog.ShowDialog(); 


            if (result == true)
            {
                ReadCSV rc = new ReadCSV();
                this.data = rc.Read(openFileDialog.FileName); //ReadCsv for filename

                this.pixelString = data.Item1; //For The List Box
                this.RamanShiftString = data.Item2; //For The List Box
                this.yString = data.Item3; //For The ListBox

                XDisplayRamanShift();
            }

        }

        private void XDisplayRamanShiftOnCLick(object sender, RoutedEventArgs e) //When clicking back to raman x axis
        {
            XDisplayRamanShift();
        }

        private void XDisplayRamanShift()
        {
            x_GraphLabel.Text = "Raman Shift"; //Label above Coords box

            x_coordinates.ItemsSource = RamanShiftString; //ListBox item source
            y_coordinates.ItemsSource = yString; //ListBox item source

            string[] RamanShiftArray = RamanShiftString.ToArray(); //In order to convert to double
            string[] Y_axisArray = yString.ToArray(); //In order to convert to double

            x_coords = Array.ConvertAll(RamanShiftArray, double.Parse); //Double coordinates for graph
            y_coords = Array.ConvertAll(Y_axisArray, double.Parse); //Double coordinates for graph

            plot.Plot.Clear(); //Clear Current Plot
            plot.Plot.AddScatter(x_coords, y_coords);
            plot.Plot.XLabel("Raman Shift");
            plot.Plot.YLabel("Intensity");
            plot.Refresh();
        }

        private void XDisplayPixelsOnClick(object sender, RoutedEventArgs e)
        {
            x_GraphLabel.Text = "Pixels"; //Label above coords box

            x_coordinates.ItemsSource = pixelString; //ListBox item source
            y_coordinates.ItemsSource = yString; //ListBox item source

            string[] pixelArray = pixelString.ToArray(); //In order to convert to double
            string[] Y_axisArray = yString.ToArray(); //In order to convert to double

            x_coords = Array.ConvertAll(pixelArray, double.Parse); //Double coordinates for graph 
            y_coords = Array.ConvertAll(Y_axisArray, double.Parse); //Double coordinates for graph 
            plot.Plot.Clear();
            plot.Plot.AddScatter(x_coords, y_coords);
            plot.Plot.XLabel("Pixels");
            plot.Plot.YLabel("Intensity");
            plot.Refresh();

        }
    }
}