using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using ScottPlot;
using MathNet;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using Raman.CSVReading;

namespace Raman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CsvData csvData; //stores pixel, raman shift, intensity

        public MainWindow()
        {
            InitializeComponent();
        }

        public Tuple<List<string>, List<string>, List<string>> data; //Holds data returned from ReadCsv()

        private List<string> pixelString; //For The List Box
        private List<string> RamanShiftString; //For The List Box
        private List<string> yString; //For The List Box

        private double[] x_coords; //For the graph
        private double[] y_coords; //For the graph

        private void TestCsvOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); //Open File viewer
                Nullable<bool> result = openFileDialog.ShowDialog();


                if (result == true)
                {
                    ReadCSV rc = new ReadCSV();
                    rc.Read(openFileDialog.FileName); //ReadCsv for filename
                    this.csvData = rc.csvData; //instance of pixels, raman shift, and intensity

                    XDisplayRamanShift();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unsupported file.");
            }
            
        }

        private void XDisplayRamanShiftOnCLick(object sender, RoutedEventArgs e) //When clicking back to raman x axis
        {
            XDisplayRamanShift();
        }

        private void XDisplayRamanShift()
        {
            try
            {
                x_GraphLabel.Text = "Raman Shift"; //Label above Coords box

                x_coordinates.ItemsSource = csvData.RamanShift; //ListBox item source
                y_coordinates.ItemsSource = csvData.Intensity; //ListBox item source

                string[] RamanShiftArray = csvData.RamanShift.ToArray(); //In order to convert to double
                string[] Y_axisArray = csvData.Intensity.ToArray(); //In order to convert to double

                x_coords = Array.ConvertAll(RamanShiftArray, double.Parse); //Double coordinates for graph
                y_coords = Array.ConvertAll(Y_axisArray, double.Parse); //Double coordinates for graph

                plot.Plot.Clear(); //Clear Current Plot
                plot.Plot.AddScatter(x_coords, y_coords);
                plot.Plot.XLabel("Raman Shift");
                plot.Plot.YLabel("Intensity");
                plot.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must import a file first.");
            }
            
        }

        private void XDisplayPixelsOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                x_GraphLabel.Text = "Pixels"; //Label above coords box

                x_coordinates.ItemsSource = csvData.RamanShift; //ListBox item source
                y_coordinates.ItemsSource = csvData.Intensity; //ListBox item source

                string[] pixelArray = csvData.Pixels.ToArray(); //In order to convert to double
                string[] Y_axisArray = csvData.Intensity.ToArray(); //In order to convert to double

                x_coords = Array.ConvertAll(pixelArray, double.Parse); //Double coordinates for graph 
                y_coords = Array.ConvertAll(Y_axisArray, double.Parse); //Double coordinates for graph 
                plot.Plot.Clear();
                plot.Plot.AddScatter(x_coords, y_coords);
                plot.Plot.XLabel("Pixels");
                plot.Plot.YLabel("Intensity");
                plot.Refresh();
            }

            catch (Exception ex)
            {
                MessageBox.Show("You must import a file first.");
            }

        }

        private void FitOnClick(object sender, RoutedEventArgs e)
        {
            FitPopup FitWindow = new FitPopup();
            FitWindow.Show();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}