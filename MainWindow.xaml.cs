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
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Linq;

namespace Raman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public App app = ((App)App.Current);
        /*
        public CsvData csvData; //stores pixel, raman shift, intensity
        public FitParams? fitParams;
        public LinearParams linearParams;
        public List<GaussianParams> gaussianParamsList = new List<GaussianParams>(); 
        public RangeConvert range;
        public CurveFit? curveFit;
        public List<double> areas = new List<double>();
        public List<double> ratios = new List<double>();
        */
        public MainWindow()
        {
            InitializeComponent();

            Conditions.Visibility = Visibility.Collapsed;
            Results.Visibility = Visibility.Collapsed;
            Boundaries.Visibility = Visibility.Collapsed;
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
                    app.model.csvData = rc.csvData; //instance of pixels, raman shift, and intensity

                    app.model.areas.Clear();
                    app.model.ratios.Clear();
                    try
                    {
                        app.model.curveFit.gaussianFit.Clear();
                    }
                    catch
                    {

                    }

                    XDisplayRamanShift(true);
                }
            }
            catch (Exception ex)
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
                x_coordinates.ItemsSource = app.model.csvData.RamanShift; //ListBox item source
                y_coordinates.ItemsSource = app.model.csvData.Intensity; //ListBox item source

                string[] RamanShiftArray = app.model.csvData.RamanShift.ToArray(); //In order to convert to double
                string[] Y_axisArray = app.model.csvData.Intensity.ToArray(); //In order to convert to double

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
                        plot.Plot.AddScatter(app.model.range.ramanShift, app.model.curveFit.fitOutput);
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

                x_coordinates.ItemsSource = app.model.csvData.RamanShift; //ListBox item source
                y_coordinates.ItemsSource = app.model.csvData.Pixels; //ListBox item source

                string[] pixelArray = app.model.csvData.Pixels.ToArray(); //In order to convert to double
                string[] Y_axisArray = app.model.csvData.Intensity.ToArray(); //In order to convert to double

                x_coords = Array.ConvertAll(pixelArray, double.Parse); //Double coordinates for graph 
                y_coords = Array.ConvertAll(Y_axisArray, double.Parse); //Double coordinates for graph 
                plot.Plot.Clear();
                plot.Plot.AddScatter(x_coords, y_coords);
                try
                {
                    plot.Plot.AddScatter(app.model.range.pixels, app.model.curveFit.fitOutput);
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
            FitPopup FitWindow = new FitPopup(this);

            bool? saved = FitWindow.ShowDialog(); //open fit settings window

            if (saved == false)
            {
                app.model.fitParams = FitWindow.fitparams;
                FitConditions();
            }
        }

        public void FitConditions()
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
                FitType.Text = "Fit: " + app.model.fitParams.Fit;
                X_Range.Text = "Range: " + app.model.fitParams.Min + " - " + app.model.fitParams.Max;
                if (app.model.fitParams.Fit == "Single")
                {
                    DoubleGaus.Visibility = Visibility.Collapsed;
                    Triple_Gaus.Visibility = Visibility.Collapsed;
                }
                if (app.model.fitParams.Fit == "Double")
                {
                    DoubleGaus.Visibility = Visibility.Visible;
                    Triple_Gaus.Visibility = Visibility.Collapsed;
                }
                if (app.model.fitParams.Fit == "Triple")
                {
                    DoubleGaus.Visibility = Visibility.Visible;
                    Triple_Gaus.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                Conditions.Visibility = Visibility.Collapsed;
            }

            if (app.model.gaussianParamsList.Count > 0)
            {
                Slope.Text = app.model.linearParams.Slope.ToString();
                Intercept.Text = app.model.linearParams.Intercept.ToString();

                for (int i = 0; i < app.model.gaussianParamsList.Count; i++)
                {
                    if (i == 0)
                    {
                        Amplitude_one.Text = app.model.gaussianParamsList[i].Amplitude.ToString();
                        μ_one.Text = app.model.gaussianParamsList[i].Center.ToString();
                        σ_one.Text = app.model.gaussianParamsList[i].SD.ToString();
                    }
                    if (i == 1)
                    {
                        Amplitude_two.Text = app.model.gaussianParamsList[i].Amplitude.ToString();
                        μ_two.Text = app.model.gaussianParamsList[i].Center.ToString();
                        σ_two.Text = app.model.gaussianParamsList[i].SD.ToString();
                    }
                    if (i == 2)
                    {
                        Amplitude_three.Text = app.model.gaussianParamsList[i].Amplitude.ToString();
                        μ_three.Text = app.model.gaussianParamsList[i].Center.ToString();
                        σ_three.Text = app.model.gaussianParamsList[i].SD.ToString();
                    }
                }
            }
        }

        public void BoundaryConditions()
        {
            Slope_upper.Text = "";
            Slope_lower.Text = "";
            Intercept_upper.Text = "";
            Intercept_lower.Text = "";
            Amplitude_one_upper.Text = "";
            Amplitude_one_lower.Text = "";
            μ_one_upper.Text = "";
            μ_one_lower.Text = "";
            σ_one_upper.Text = "";
            σ_one_lower.Text = "";
            Amplitude_two_upper.Text = "";
            Amplitude_two_lower.Text = "";
            μ_two_upper.Text = "";
            μ_two_lower.Text = "";
            σ_two_upper.Text = "";
            σ_two_lower.Text = "";
            Amplitude_three_upper.Text = "";
            Amplitude_three_lower.Text = "";
            μ_three_upper.Text = "";
            μ_three_lower.Text = "";
            σ_three_upper.Text = "";
            σ_three_lower.Text = "";

            if (app.model.fitParams.Fit == "Single")
            {
                Double_Gaus_Boundary.Visibility = Visibility.Collapsed;
                Triple_Gaus_Boundary.Visibility = Visibility.Collapsed;
            }
            if (app.model.fitParams.Fit == "Double")
            {
                Double_Gaus_Boundary.Visibility = Visibility.Visible;
                Triple_Gaus_Boundary.Visibility = Visibility.Collapsed;
            }
            if (app.model.fitParams.Fit == "Triple")
            {
                Double_Gaus_Boundary.Visibility = Visibility.Visible;
                Triple_Gaus_Boundary.Visibility = Visibility.Visible;
            }


            if (app.model.gaussianParamsList.Count > 0)
            {
                Slope_upper.Text = app.model.linearBoundaryConditions.sloper_upper.ToString();
                Slope_lower.Text = app.model.linearBoundaryConditions.sloper_lower.ToString();
                Intercept_upper.Text = app.model.linearBoundaryConditions.intercept_upper.ToString();
                Intercept_lower.Text = app.model.linearBoundaryConditions.intercept_lower.ToString();

                for (int i = 0; i < app.model.gaussianParamsList.Count; i++)
                {
                    if (i == 0)
                    {
                        Amplitude_one_upper.Text = app.model.boundaryConditionsList[i].amplitude_upper.ToString();
                        Amplitude_one_lower.Text = app.model.boundaryConditionsList[i].amplitude_lower.ToString();
                        μ_one_upper.Text = app.model.boundaryConditionsList[i].center_upper.ToString();
                        μ_one_lower.Text = app.model.boundaryConditionsList[i].center_lower.ToString();
                        σ_one_upper.Text = app.model.boundaryConditionsList[i].sd_upper.ToString();
                        σ_one_lower.Text = app.model.boundaryConditionsList[i].sd_lower.ToString();
                    }
                    if (i == 1)
                    {
                        Amplitude_two_upper.Text = app.model.boundaryConditionsList[i].amplitude_upper.ToString();
                        Amplitude_two_lower.Text = app.model.boundaryConditionsList[i].amplitude_lower.ToString();
                        μ_two_upper.Text = app.model.boundaryConditionsList[i].center_upper.ToString();
                        μ_two_lower.Text = app.model.boundaryConditionsList[i].center_lower.ToString();
                        σ_two_upper.Text = app.model.boundaryConditionsList[i].sd_upper.ToString();
                        σ_two_lower.Text = app.model.boundaryConditionsList[i].sd_lower.ToString();
                    }
                    if (i == 2)
                    {
                        Amplitude_three_upper.Text = app.model.boundaryConditionsList[i].amplitude_upper.ToString();
                        Amplitude_three_lower.Text = app.model.boundaryConditionsList[i].amplitude_lower.ToString();
                        μ_three_upper.Text = app.model.boundaryConditionsList[i].center_upper.ToString();
                        μ_three_lower.Text = app.model.boundaryConditionsList[i].center_lower.ToString();
                        σ_three_upper.Text = app.model.boundaryConditionsList[i].sd_upper.ToString();
                        σ_three_lower.Text = app.model.boundaryConditionsList[i].sd_lower.ToString();
                    }
                }
            }

        }

        private void CurveFitWithoutBoundariesOnClick(object sender, RoutedEventArgs e)
        {
            app.model.boundaryFlag = false;
            FitCurve();
        }

        private void CurveFitWithBoundaries(object sender, RoutedEventArgs e)
        {
            app.model.boundaryFlag = true;
            FitCurve();
        }

        private void FitCurve()
        {
            try
            {
                app.model.curveFit = null;

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

                if (app.model.boundaryFlag == true)
                {
                    LinearBoundaryConditions linearBoundaryConditions = new LinearBoundaryConditions(double.Parse(Slope_upper.Text), double.Parse(Slope_lower.Text), double.Parse(Intercept_upper.Text), double.Parse(Intercept_lower.Text));
                    List<BoundaryConditions> boundaryConditionsList = new List<BoundaryConditions>();

                    BoundaryConditions boudaryConditionsOne = new BoundaryConditions(double.Parse(Amplitude_one_upper.Text), double.Parse(Amplitude_one_lower.Text), double.Parse(μ_one_upper.Text), double.Parse(μ_one_lower.Text), double.Parse(σ_one_upper.Text), double.Parse(σ_one_lower.Text));
                    boundaryConditionsList.Add(boudaryConditionsOne);
                    if (Amplitude_two.Text.Length != 0)
                    {
                        BoundaryConditions boudaryConditionsTwo = new BoundaryConditions(double.Parse(Amplitude_two_upper.Text), double.Parse(Amplitude_two_lower.Text), double.Parse(μ_two_upper.Text), double.Parse(μ_two_lower.Text), double.Parse(σ_two_upper.Text), double.Parse(σ_two_lower.Text));
                        boundaryConditionsList.Add(boudaryConditionsTwo);


                    }
                    if (Amplitude_three.Text.Length != 0)
                    {
                        BoundaryConditions boudaryConditionsThree = new BoundaryConditions(double.Parse(Amplitude_three_upper.Text), double.Parse(Amplitude_three_lower.Text), double.Parse(μ_three_upper.Text), double.Parse(μ_three_lower.Text), double.Parse(σ_three_upper.Text), double.Parse(σ_three_lower.Text));
                        boundaryConditionsList.Add((boudaryConditionsThree));
                    }
                    app.model.boundaryConditionsList = boundaryConditionsList;
                    app.model.linearBoundaryConditions = linearBoundaryConditions;
                }

                    RangeConvert rangeConvert = new RangeConvert(app.model.csvData, app.model.fitParams);
                    app.model.range = rangeConvert;
                    app.model.gaussianParamsList = paramsGaussianInitialConditionsList;
                    app.model.linearParams = linearParamsInitalConditions;

                }

            catch
            {
                MessageBox.Show("Please supply ALL parameters for the fit", "Could not fit");
                return;
            }

            try
            {
                app.model.curveFit = new CurveFit(app.model.range.pixels, app.model.range.intensity, app.model.range.ramanShift, app.model.fitParams);
                string error = app.model.curveFit.RunFit(app.model.linearParams, app.model.gaussianParamsList, app.model.boundaryConditionsList, app.model.linearBoundaryConditions, app.model.range.ramanShift, app.model.range.intensity, app.model.boundaryFlag);

                MessageBox.Show(error.ToString(), "Fit least squares error.");
                plot.Plot.Clear();
                plot.Plot.AddScatter(x_coords, y_coords);
                plot.Plot.AddScatter(app.model.range.ramanShift, app.model.curveFit.fitOutput);
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
            textList.Add("fit[" + app.model.fitParams.Fit + "]");
            textList.Add("min[" + app.model.fitParams.Min + "]");
            textList.Add("max[" + app.model.fitParams.Max + "]");
            textList.Add("slope[" + app.model.linearParams.Slope + "]");
            textList.Add("slope_upper[" + app.model.linearBoundaryConditions.sloper_upper + "]");
            textList.Add("slope_lower[" + app.model.linearBoundaryConditions.sloper_lower + "]");
            textList.Add("intercept[" + app.model.linearParams.Intercept + "]");
            textList.Add("intercept_upper[" + app.model.linearBoundaryConditions.sloper_upper + "]");
            textList.Add("intercept_lower[" + app.model.linearBoundaryConditions.sloper_lower + "]");

            for (int i = 0; i < app.model.gaussianParamsList.Count; i++)
            {
                textList.Add("amplitude" + i + "[" + app.model.gaussianParamsList[i].Amplitude + "]");
                textList.Add("amplitude_upper" + i + "[" + app.model.boundaryConditionsList[i].amplitude_upper + "]");
                textList.Add("amplitude_lower" + i + "[" + app.model.boundaryConditionsList[i].amplitude_lower + "]");
                textList.Add("center" + i + "[" + app.model.gaussianParamsList[i].Center + "]");
                textList.Add("center_upper" + i + "[" + app.model.boundaryConditionsList[i].center_upper + "]");
                textList.Add("center_lower" + i + "[" + app.model.boundaryConditionsList[i].center_lower + "]");
                textList.Add("sd" + i + "[" + app.model.gaussianParamsList[i].SD + "]");
                textList.Add("sd_upper" + i + "[" + app.model.boundaryConditionsList[i].sd_upper + "]");
                textList.Add("sd_upper" + i + "[" + app.model.boundaryConditionsList[i].sd_lower + "]");
            }

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Fit_Profile.txt";
            save.Filter = "Text File | *.txt";
            if ((bool)save.ShowDialog()) ;
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
            app.model.gaussianParamsList = new List<GaussianParams>();
            app.model.boundaryConditionsList = new List<BoundaryConditions>();
            try
            {
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
                        app.model.fitParams = new FitParams(rg.Match(f[1]).ToString(), double.Parse(rg.Match(f[2]).ToString()), double.Parse(rg.Match(f[3]).ToString()));
                        app.model.linearParams = new LinearParams(double.Parse(rg.Match(f[4]).ToString()), double.Parse(rg.Match(f[7]).ToString()));
                        app.model.linearBoundaryConditions = new LinearBoundaryConditions(double.Parse(rg.Match(f[5]).ToString()), double.Parse(rg.Match(f[6]).ToString()), double.Parse(rg.Match(f[8]).ToString()), double.Parse(rg.Match(f[9]).ToString()));
                        app.model.gaussianParamsList.Add(new GaussianParams(double.Parse(rg.Match(f[10]).ToString()), double.Parse(rg.Match(f[13]).ToString()), double.Parse(rg.Match(f[16]).ToString())));
                        app.model.boundaryConditionsList.Add(new BoundaryConditions(double.Parse(rg.Match(f[11]).ToString()), double.Parse(rg.Match(f[12]).ToString()), double.Parse(rg.Match(f[14]).ToString()), double.Parse(rg.Match(f[15]).ToString()), double.Parse(rg.Match(f[17]).ToString()), double.Parse(rg.Match(f[18]).ToString())));
                        if (f.Count > 18)
                        {
                            app.model.gaussianParamsList.Add(new GaussianParams(double.Parse(rg.Match(f[19]).ToString()), double.Parse(rg.Match(f[22]).ToString()), double.Parse(rg.Match(f[25]).ToString())));
                            app.model.boundaryConditionsList.Add(new BoundaryConditions(double.Parse(rg.Match(f[20]).ToString()), double.Parse(rg.Match(f[21]).ToString()), double.Parse(rg.Match(f[23]).ToString()), double.Parse(rg.Match(f[24]).ToString()), double.Parse(rg.Match(f[26]).ToString()), double.Parse(rg.Match(f[27]).ToString())));
                            if (f.Count > 28)
                            {
                                app.model.gaussianParamsList.Add(new GaussianParams(double.Parse(rg.Match(f[28]).ToString()), double.Parse(rg.Match(f[31]).ToString()), double.Parse(rg.Match(f[34]).ToString())));
                                app.model.boundaryConditionsList.Add(new BoundaryConditions(double.Parse(rg.Match(f[29]).ToString()), double.Parse(rg.Match(f[30]).ToString()), double.Parse(rg.Match(f[32]).ToString()), double.Parse(rg.Match(f[33]).ToString()), double.Parse(rg.Match(f[35]).ToString()), double.Parse(rg.Match(f[36]).ToString())));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Unsupported File");
                    }
                    XDisplayRamanShift(true);
                    FitConditions();
                    BoundaryConditions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unsupported File.");
            }
        }

        private void Conditions_View_OnClick(object sender, RoutedEventArgs e)
        {
            Conditions.Visibility = Visibility.Visible;
            Results.Visibility = Visibility.Collapsed;
            Boundaries.Visibility = Visibility.Collapsed;
            ConditionsView.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#ed9f4b");
            ResultsView.Background = Brushes.Gray;
            Boundary_Conditions_button.Background = Brushes.Gray;
        }

        private void Boundary_Conditions_OnClick(object sender, RoutedEventArgs e)
        {
            Boundaries.Visibility = Visibility.Visible;
            Conditions.Visibility = Visibility.Collapsed;
            Results.Visibility = Visibility.Collapsed;
            ResultsView.Background = Brushes.Gray;
            ConditionsView.Background = Brushes.Gray;
            Boundary_Conditions_button.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#ed9f4b");
        }

        private void Results_View_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (app.model.areas.Count == 0)
                {
                    AreaOne.Visibility = Visibility.Collapsed;
                    AreaTwo.Visibility = Visibility.Collapsed;
                    AreaThree.Visibility = Visibility.Collapsed;
                    Ratios.Visibility = Visibility.Collapsed;
                }
                else if (app.model.areas.Count == 1)
                {
                    AreaOne.Visibility = Visibility.Visible;
                }
                else if (app.model.areas.Count == 2)
                {
                    AreaOne.Visibility = Visibility.Visible;
                    AreaTwo.Visibility = Visibility.Visible;
                    Ratios.Visibility = Visibility.Visible;
                }
                else if (app.model.areas.Count == 3)
                {
                    AreaOne.Visibility = Visibility.Visible;
                    AreaTwo.Visibility = Visibility.Visible;
                    AreaThree.Visibility = Visibility.Visible;
                }
                if (app.model.curveFit.gaussianFit.Count != 0)
                {
                    ResultsView.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#ed9f4b");
                    ConditionsView.Background = Brushes.Gray;
                    Boundary_Conditions_button.Background = Brushes.Gray;
                    Results.Visibility = Visibility.Visible;
                    Conditions.Visibility = Visibility.Collapsed;
                    Boundaries.Visibility = Visibility.Collapsed;

                    Slope_results.Text = app.model.curveFit.baselineFit.Slope.ToString();
                    Intercept_results.Text = app.model.curveFit.baselineFit.Intercept.ToString();

                    for (int i = 0; i < app.model.curveFit.gaussianFit.Count; i++)
                    {
                        if (i == 0)
                        {
                            Gaussian_results_two.Visibility = Visibility.Collapsed;
                            Gaussian_results_three.Visibility = Visibility.Collapsed;
                            Amplitude_results.Text = app.model.curveFit.gaussianFit[i].Amplitude.ToString();
                            μ_results.Text = app.model.curveFit.gaussianFit[i].Center.ToString();
                            σ_results.Text = app.model.curveFit.gaussianFit[i].SD.ToString();
                        }
                        if (i == 1)
                        {
                            Gaussian_results_two.Visibility = Visibility.Visible;
                            Gaussian_results_three.Visibility = Visibility.Collapsed;
                            amplitude_results_two.Text = app.model.curveFit.gaussianFit[i].Amplitude.ToString();
                            μ_results_two.Text = app.model.curveFit.gaussianFit[i].Center.ToString();
                            σ_results_two.Text = app.model.curveFit.gaussianFit[i].SD.ToString();
                        }
                        if (i == 2)
                        {
                            Gaussian_results_two.Visibility = Visibility.Visible;
                            Gaussian_results_three.Visibility = Visibility.Visible;
                            amplitude_results_three.Text = app.model.curveFit.gaussianFit[i].Amplitude.ToString();
                            μ_results_three.Text = app.model.curveFit.gaussianFit[i].Center.ToString();
                            σ_results_three.Text = app.model.curveFit.gaussianFit[i].SD.ToString();
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
            app.model.areas.Clear();
            app.model.ratios.Clear();
            /*
                Ratios #0 = 1/2 or 2/1
                Ratios #1 = 2/3 or 3/2
                Ratios #2 = 1/3 or 3/1
             */

            app.model.areas.Clear();
            if (app.model.curveFit.gaussianFit.Count == 1)
            {
                AreaOne.Visibility = Visibility.Visible;
                app.model.areas.Add(IntegrateGauss(app.model.curveFit.gaussianFit[0]));
                GaussianOneArea.Text = app.model.areas[0].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios: None";
                G01_Order.Visibility = Visibility.Collapsed;
                G01_Result.Visibility = Visibility.Collapsed;
                G02_Order.Visibility = Visibility.Collapsed;
                G02_Result.Visibility = Visibility.Collapsed;
                G12_Order.Visibility = Visibility.Collapsed;
                G12_Result.Visibility = Visibility.Collapsed;
            }
            if (app.model.curveFit.gaussianFit.Count == 2)
            {
                AreaOne.Visibility = Visibility.Visible;
                AreaTwo.Visibility = Visibility.Visible;
                app.model.areas.Add(IntegrateGauss(app.model.curveFit.gaussianFit[0]));
                app.model.areas.Add(IntegrateGauss(app.model.curveFit.gaussianFit[1]));
                GaussianOneArea.Text = app.model.areas[0].ToString("0.00");
                GaussianTwoArea.Text = app.model.areas[1].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                G01_Order.Visibility = Visibility.Visible;
                G01_Result.Visibility = Visibility.Visible;
                G02_Order.Visibility = Visibility.Collapsed;
                G02_Result.Visibility = Visibility.Collapsed;
                G12_Order.Visibility = Visibility.Collapsed;
                G12_Result.Visibility = Visibility.Collapsed;
                RatiosDisplay.Text = "Ratios:";

                if (app.model.areas[0] > app.model.areas[1])
                {
                    G01.Visibility = Visibility.Visible;
                    G01_Order.Text = "G1:G2";
                    app.model.ratios.Add(app.model.areas[0] / app.model.areas[1]);
                    G01_Result.Text = app.model.ratios[0].ToString();
                }
                else
                {
                    G01.Visibility = Visibility.Visible;
                    G01_Order.Text = "G1:G2";
                    app.model.ratios.Add(app.model.areas[1] / app.model.areas[0]);
                    G01_Result.Text = app.model.ratios[0].ToString();
                }
            }
            if (app.model.curveFit.gaussianFit.Count == 3)
            {
                AreaOne.Visibility = Visibility.Visible;
                AreaTwo.Visibility = Visibility.Visible;
                AreaThree.Visibility = Visibility.Visible;
                app.model.areas.Add(IntegrateGauss(app.model.curveFit.gaussianFit[0]));
                app.model.areas.Add(IntegrateGauss(app.model.curveFit.gaussianFit[1]));
                app.model.areas.Add(IntegrateGauss(app.model.curveFit.gaussianFit[2]));
                GaussianOneArea.Text = app.model.areas[0].ToString("0.00");
                GaussianTwoArea.Text = app.model.areas[1].ToString("0.00");
                GaussianThreeArea.Text = app.model.areas[2].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                G01_Order.Visibility = Visibility.Visible;
                G01_Result.Visibility = Visibility.Visible;
                G02_Order.Visibility = Visibility.Visible;
                G02_Result.Visibility = Visibility.Visible;
                G12_Order.Visibility = Visibility.Visible;
                G12_Result.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios:";
                if (app.model.areas[0] > app.model.areas[1])
                {
                    G01.Visibility = Visibility.Visible;
                    G01_Order.Text = "G1:G2";
                    app.model.ratios.Add(app.model.areas[0] / app.model.areas[1]);
                    G01_Result.Text = app.model.ratios[0].ToString();
                }
                else
                {
                    G01.Visibility = Visibility.Visible;
                    G01_Order.Text = "G2:G1";
                    app.model.ratios.Add(app.model.areas[1] / app.model.areas[0]);
                    G01_Result.Text = app.model.ratios[0].ToString();
                }
                if (app.model.areas[0] > app.model.areas[2])
                {
                    G02.Visibility = Visibility.Visible;
                    G02_Order.Text = "G1:G3";
                    app.model.ratios.Add(app.model.areas[0] / app.model.areas[2]);
                    G02_Result.Text = app.model.ratios[1].ToString();
                }
                else
                {
                    G02.Visibility = Visibility.Visible;
                    G02_Order.Text = "G3:G1";
                    app.model.ratios.Add(app.model.areas[2] / app.model.areas[0]);
                    G02_Result.Text = app.model.ratios[1].ToString();
                }
                if (app.model.areas[2] > app.model.areas[1])
                {
                    G12.Visibility = Visibility.Visible;
                    G12_Order.Text = "G3:G2";
                    app.model.ratios.Add(app.model.areas[2] / app.model.areas[1]);
                    G12_Result.Text = app.model.ratios[2].ToString();
                }
                else
                {
                    G12.Visibility = Visibility.Visible;
                    G12_Order.Text = "G2:G3";
                    app.model.ratios.Add(app.model.areas[1] / app.model.areas[2]);
                    G12_Result.Text = app.model.ratios[2].ToString();
                }
            }
        }

        private double IntegrateGauss(GaussianParams p)
        {
            Integrate integrate = new Integrate(p);
            double integral = integrate.run();
            return integral;
        }
    }
}