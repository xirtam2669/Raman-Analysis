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
using Raman.Fitting;

namespace Raman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CsvData csvData; //stores pixel, raman shift, intensity
        public FitParams fitParams;
        GaussianParams gaussianParams;
        RangeConvert rangeConvert;

        public MainWindow()
        {
            InitializeComponent();
            Conditions.Visibility = Visibility.Collapsed; 
            DoubleGaus.Visibility = Visibility.Collapsed;
            Triple_Gaus.Visibility = Visibility.Collapsed;
        }
        
        public Tuple<List<string>, List<string>, List<string>> data; //Holds data returned from ReadCsv()

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
                x_GraphLabel.Text = "Raman Shift";
                y_GraphLabel.Text = "Dark Subtracted";
                x_coordinates.ItemsSource = csvData.RamanShift; //ListBox item source
                y_coordinates.ItemsSource = csvData.Intensity; //ListBox item source

                string[] RamanShiftArray = csvData.RamanShift.ToArray(); //In order to convert to double
                string[] Y_axisArray = csvData.Intensity.ToArray(); //In order to convert to double

                x_coords = Array.ConvertAll(RamanShiftArray, double.Parse); //Double coordinates for graph
                y_coords = Array.ConvertAll(Y_axisArray, double.Parse); //Double coordinates for graph

                plot.Plot.Clear(); //Clear Current Plot
                plot.Plot.AddScatter(x_coords, y_coords);

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
                y_GraphLabel.Text = "Pixels"; //Label above coords box

                x_coordinates.ItemsSource = csvData.RamanShift; //ListBox item source
                y_coordinates.ItemsSource = csvData.Pixels; //ListBox item source

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
            FitWindow.ShowDialog();
            this.fitParams = FitWindow.fitparams;
            FitConditions();
        }

        private void FitConditions()
        {
            Conditions.Visibility = Visibility.Visible; //will be important probably
            FitType.Text = "Fit: " + fitParams.Fit;
            if(fitParams.Fit == "Single")
            {
                DoubleGaus.Visibility = Visibility.Collapsed;
                Triple_Gaus.Visibility = Visibility.Collapsed;
            }
            if (fitParams.Fit == "Double")
            {
                DoubleGaus.Visibility = Visibility.Visible;
            }
            if(fitParams.Fit == "Triple")
            {
                Triple_Gaus.Visibility = Visibility.Visible;
            }
            
            X_Range.Text = "Range: " + fitParams.Min + " - " + fitParams.Max;
            
        }

        private void CurveFitOnClick(object sender, RoutedEventArgs e)
        {
            LinearParams linearparams = new LinearParams(double.Parse(Slope.Text), double.Parse(Intercept.Text));
            List <GaussianParams> paramsGaussianInitialConditionsList = new List<GaussianParams>();

            GaussianParams gaussianParamsOne = new GaussianParams(Double.Parse(Amplitude_one.Text), Double.Parse(μ_one.Text), Double.Parse(σ_one.Text));
            paramsGaussianInitialConditionsList.Add(gaussianParamsOne);
            if (Amplitude_two.Text.Length != 0)
            {
                GaussianParams gaussianParams2 = new GaussianParams(Double.Parse(Amplitude_two.Text), Double.Parse(μ_two.Text), Double.Parse(σ_two.Text));
                paramsGaussianInitialConditionsList.Add(gaussianParams2);
            }
            if (Amplitude_three.Text.Length != 0)
            {
                GaussianParams gaussianParams3 = new GaussianParams(Double.Parse(Amplitude_three.Text), Double.Parse(μ_three.Text), Double.Parse(σ_three.Text));
                paramsGaussianInitialConditionsList.Add(gaussianParams3);
            }

            RangeConvert rangeConvert = new RangeConvert(csvData, fitParams);  

            CurveFit curveFit = new CurveFit(rangeConvert.pixels, rangeConvert.intensity, rangeConvert.ramanShift, fitParams);
            double[] error = curveFit.RunFit(linearparams, paramsGaussianInitialConditionsList, rangeConvert.ramanShift, rangeConvert.intensity);
            
            MessageBox.Show(error.ToString());
            plot.Plot.Clear();
            plot.Plot.AddScatter(x_coords, y_coords);
            plot.Plot.AddScatter(rangeConvert.ramanShift, curveFit.fitOutput);
            plot.Refresh();
        }
    }
}