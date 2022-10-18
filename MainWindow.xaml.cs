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
using System.IO;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Media.TextFormatting;
using System.Net;

namespace Raman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public App app = ((App)App.Current);
        public CsvData csvData; //stores pixel, raman shift, intensity
        public FitParams? fitParams;
        public LinearParams linearParams;
        public List<GaussianParams> gaussianParamsList = new List<GaussianParams>();
        public RangeConvert range;
        public CurveFit? curveFit;
        public List<double> areas = new List<double>();
        public List<double> ratios = new List<double>();

        public MainWindow()
        {
            InitializeComponent();
            
            Conditions.Visibility = Visibility.Collapsed; 
            Results.Visibility = Visibility.Collapsed;
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

                    XDisplayRamanShift(true);
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

        private void XDisplayRamanShift(bool newCsv = false)
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
                try
                {
                    if (newCsv)
                    {

                    }
                    else
                    {
                        plot.Plot.AddScatter(range.ramanShift, curveFit.fitOutput);
                    }                 
                }
                catch
                {

                }
                plot.Plot.YLabel("Intensity (arb)");
                plot.Plot.XLabel("Raman Shift cm−1");
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
                try 
                {
                    plot.Plot.AddScatter(range.pixels, curveFit.fitOutput);
                }
                catch
                {

                }
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
            gaussianParamsList = new List<GaussianParams>(); //clear gaussian
            FitPopup FitWindow = new FitPopup();

            FitWindow.ShowDialog(); //open fit settings window

            this.fitParams = FitWindow.fitparams;
            FitConditions();
         
        }

        private void FitConditions()
        {
            try
            {
                Slope.Text = "";
                Intercept.Text = "";
                Amplitude_one.Text = "";
                μ_one.Text = "";
                σ_one.Text = "";
                Amplitude_two.Text = "";
                μ_two.Text = "";
                σ_two.Text = "";
                Amplitude_three.Text = "";
                μ_three.Text = "";
                σ_three.Text = "";
                Results.Visibility = Visibility.Collapsed;
                Conditions.Visibility = Visibility.Visible;
                FitType.Text = "Fit: " + fitParams.Fit;
                X_Range.Text = "Range: " + fitParams.Min + " - " + fitParams.Max;
                if (fitParams.Fit == "Single")
                {
                    DoubleGaus.Visibility = Visibility.Collapsed;
                    Triple_Gaus.Visibility = Visibility.Collapsed;
                }
                if (fitParams.Fit == "Double")
                {
                    DoubleGaus.Visibility = Visibility.Visible;
                    Triple_Gaus.Visibility = Visibility.Collapsed;
                }
                if (fitParams.Fit == "Triple")
                {
                    DoubleGaus.Visibility = Visibility.Visible;
                    Triple_Gaus.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                Conditions.Visibility = Visibility.Collapsed;
            }

            if (gaussianParamsList.Count > 0)
            {
                Slope.Text = linearParams.Slope.ToString();
                Intercept.Text = linearParams.Intercept.ToString();
            
                for (int i = 0; i < gaussianParamsList.Count; i++)
                {
                    if (i == 0)
                    {
                        Amplitude_one.Text = gaussianParamsList[i].Amplitude.ToString();
                        μ_one.Text = gaussianParamsList[i].Center.ToString();
                        σ_one.Text = gaussianParamsList[i].SD.ToString();
                    }
                    if (i == 1)
                    {
                        Amplitude_two.Text = gaussianParamsList[i].Amplitude.ToString();
                        μ_two.Text = gaussianParamsList[i].Center.ToString();
                        σ_two.Text = gaussianParamsList[i].SD.ToString();
                    }
                    if (i == 2)
                    {
                        Amplitude_three.Text = gaussianParamsList[i].Amplitude.ToString();
                        μ_three.Text = gaussianParamsList[i].Center.ToString();
                        σ_three.Text = gaussianParamsList[i].SD.ToString();
                    }
                }
            }
        }

        private void CurveFitOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                curveFit = null;
                if(fitParams == null || linearParams == null)
                {
                    LinearParams linearParamsInitalConditions = new LinearParams(double.Parse(Slope.Text), double.Parse(Intercept.Text));
                    List<GaussianParams> paramsGaussianInitialConditionsList = new List<GaussianParams>();

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

                    this.range = rangeConvert;
                    this.gaussianParamsList = paramsGaussianInitialConditionsList;
                    this.linearParams = linearParamsInitalConditions;
                }
                else
                {
                    this.range = new RangeConvert(csvData, fitParams);
                }               
            }    
            catch
            {
                MessageBox.Show("Please supply ALL parameters for the fit", "Could not fit");
                return;
            }

            try
            {
                this.curveFit = new CurveFit(range.pixels, range.intensity, range.ramanShift, fitParams);
                string error = curveFit.RunFit(linearParams, gaussianParamsList, range.ramanShift, range.intensity);

                MessageBox.Show(error.ToString(), "Fit least squares error.");
                plot.Plot.Clear();
                plot.Plot.AddScatter(x_coords, y_coords);
                plot.Plot.AddScatter(range.ramanShift, curveFit.fitOutput);
                plot.Refresh();

            }
            catch
            {
                MessageBox.Show("Please supply ALL parameters for the fit", "Could not fit");
                return;
            }

        }
        private void SaveFitProfileOnClick(object sender, RoutedEventArgs e)
        {
            List<string> textList = new List<string>();

            textList.Add("[CONFIG]");
            textList.Add("fit[" + this.fitParams.Fit + "]");
            textList.Add("min[" + this.fitParams.Min + "]");
            textList.Add("max[" + this.fitParams.Max + "]");
            textList.Add("slope[" + this.linearParams.Slope + "]");
            textList.Add("intercept[" + this.linearParams.Intercept + "]");

            for (int i = 0; i < gaussianParamsList.Count; i++)
            {
                textList.Add("amplitude" + i + "[" + gaussianParamsList[i].Amplitude + "]");
                textList.Add("center" + i + "[" + gaussianParamsList[i].Center + "]");
                textList.Add("sd" + i + "[" + gaussianParamsList[i].SD + "]");
            }

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Fit_Profile.txt";
            save.Filter = "Text File | *.txt";
            if ((bool)save.ShowDialog());
            {
                string f = save.FileName;
                StreamWriter writer = new StreamWriter(save.OpenFile());
                for (int i = 0; i < textList.Count; i++)
                {
                    writer.WriteLine(textList[i].ToString());
                }
                writer.Dispose();
                writer.Close();
            }
        }

        private void ImportFit(object sender, RoutedEventArgs e)
        {
            gaussianParamsList = new List<GaussianParams>();
            try {
                OpenFileDialog openFileDialog = new OpenFileDialog(); //Open File viewer
                Nullable<bool> result = openFileDialog.ShowDialog();

                if (result == true)
                {
                    string pattern = @"(?<=\[).*(?=\])";
                    Regex rg = new Regex(pattern);
                    
                    
                    IEnumerable<string> temp = File.ReadLines(openFileDialog.FileName);
                    List<string> f = new List<string>(temp);
                    if (f[0] == "[CONFIG]")
                    { 
                        fitParams = new FitParams(rg.Match(f[1]).ToString(), double.Parse(rg.Match(f[2]).ToString()), double.Parse(rg.Match(f[3]).ToString()));
                        linearParams = new LinearParams(double.Parse(rg.Match(f[4]).ToString()), double.Parse(rg.Match(f[5]).ToString()));
                        gaussianParamsList.Add(new GaussianParams(double.Parse(rg.Match(f[6]).ToString()), double.Parse(rg.Match(f[7]).ToString()), double.Parse(rg.Match(f[8]).ToString())));
                        if (f.Count >= 9)
                        {
                            gaussianParamsList.Add(new GaussianParams(double.Parse(rg.Match(f[9]).ToString()), double.Parse(rg.Match(f[10]).ToString()), double.Parse(rg.Match(f[11]).ToString())));

                            if (f.Count > 12)
                            {
                                gaussianParamsList.Add(new GaussianParams(double.Parse(rg.Match(f[12]).ToString()), double.Parse(rg.Match(f[13]).ToString()), double.Parse(rg.Match(f[14]).ToString())));
                            }
                        }
                    }
                    else 
                    {
                        throw new Exception("Unsupported File");
                    }
                    XDisplayRamanShift(true);
                    FitConditions();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unsupported File.");
            }
        }

        private void Conditions_View(object sender, RoutedEventArgs e)
        {
            Conditions.Visibility = Visibility.Visible;
            Results.Visibility = Visibility.Collapsed;
        }

        private void Results_View(object sender, RoutedEventArgs e)
        {
            try
            {
                AreaOne.Visibility = Visibility.Collapsed;
                AreaTwo.Visibility = Visibility.Collapsed;
                AreaThree.Visibility = Visibility.Collapsed;
                if (curveFit.gaussianFit.Count != 0)
                {
                    Results.Visibility = Visibility.Visible;
                    Conditions.Visibility = Visibility.Collapsed;                   

                    Slope_results.Text = curveFit.baselineFit.Slope.ToString();
                    Intercept_results.Text = curveFit.baselineFit.Intercept.ToString();

                    for (int i = 0; i < curveFit.gaussianFit.Count; i++)
                    {
                        if (i == 0)
                        {
                            Gaussian_results_two.Visibility = Visibility.Collapsed;
                            Gaussian_results_three.Visibility = Visibility.Collapsed;
                            Amplitude_results.Text = curveFit.gaussianFit[i].Amplitude.ToString();
                            μ_results.Text = curveFit.gaussianFit[i].Center.ToString();
                            σ_results.Text = curveFit.gaussianFit[i].SD.ToString();
                        }
                        if (i == 1)
                        {
                            Gaussian_results_two.Visibility = Visibility.Visible;
                            Gaussian_results_three.Visibility = Visibility.Collapsed;
                            amplitude_results_two.Text = curveFit.gaussianFit[i].Amplitude.ToString();
                            μ_results_two.Text = curveFit.gaussianFit[i].Center.ToString();
                            σ_results_two.Text = curveFit.gaussianFit[i].SD.ToString();
                        }
                        if (i == 2)
                        {
                            Gaussian_results_two.Visibility = Visibility.Visible;
                            Gaussian_results_three.Visibility = Visibility.Visible;
                            amplitude_results_three.Text = curveFit.gaussianFit[i].Amplitude.ToString();
                            μ_results_three.Text = curveFit.gaussianFit[i].Center.ToString();
                            σ_results_three.Text = curveFit.gaussianFit[i].SD.ToString();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No results to display.");
                }
            }
            catch
            {
                MessageBox.Show("No results to display.");
            } 
        }

        private void CalculatePeakAreaRatio(object sender, RoutedEventArgs e)
        {
            /*
                Ratios #0 = 1/2 or 2/1
                Ratios #1 = 2/3 or 3/2
                Ratios #2 = 1/3 or 3/1
             */


            areas.Clear();
            if(curveFit.gaussianFit.Count == 1)
            {
                AreaOne.Visibility = Visibility.Visible;
                areas.Add(IntegrateGauss(curveFit.gaussianFit[0]));
                GaussianOneArea.Text = areas[0].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios: None";
            }
            if(curveFit.gaussianFit.Count == 2)
            {
                AreaOne.Visibility = Visibility.Visible;
                AreaTwo.Visibility = Visibility.Visible;
                areas.Add(IntegrateGauss(curveFit.gaussianFit[0]));
                areas.Add(IntegrateGauss(curveFit.gaussianFit[1]));
                GaussianOneArea.Text = areas[0].ToString("0.00");
                GaussianTwoArea.Text = areas[1].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios:";

                if (areas[0] > areas[1])
                {
                    ratios.Add(areas[0] / areas[1]);
                }
                else
                {
                    ratios.Add(areas[1] / areas[0]);
                }
            }
            if(curveFit.gaussianFit.Count == 3)
            {
                AreaOne.Visibility = Visibility.Visible;
                AreaTwo.Visibility = Visibility.Visible;
                AreaThree.Visibility = Visibility.Visible;
                areas.Add(IntegrateGauss(curveFit.gaussianFit[0]));
                areas.Add(IntegrateGauss(curveFit.gaussianFit[1]));
                areas.Add(IntegrateGauss(curveFit.gaussianFit[2]));
                GaussianOneArea.Text = areas[0].ToString("0.00");
                GaussianTwoArea.Text = areas[1].ToString("0.00");
                GaussianThreeArea.Text = areas[2].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios:";
                if (areas[0] > areas[1])
                {
                    ratios.Add(areas[0] / areas[1]);
                }
                else
                {
                    ratios.Add(areas[1] / areas[0]);
                }
                if (areas[1] > areas[2])
                {
                    ratios.Add(areas[1] / areas[2]);
                }
                else
                {
                    ratios.Add(areas[2] / areas[1]);
                }
                if (areas[3] > areas[1])
                {
                    ratios.Add(areas[3] / areas[1]);
                }
                else
                {
                    ratios.Add(areas[1] / areas[3]);
                }
            }
        }

        private double IntegrateGauss(GaussianParams p)
        {
            Integrate integrate = new Integrate(p);
            double integral = integrate.run();
            MessageBox.Show(integral.ToString());
            return integral;
        }
    }
}