using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Raman.Fitting;

namespace Raman
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FitPopup : Window
    {
        public App app = ((App)App.Current);
        string fit = "Single";
        string min;
        string max;
        public FitParams fitparams;
        private MainWindow mainWindow;

        public FitPopup(MainWindow mainWindow)
        {
            InitializeComponent();
            FitMenu.Header = "Single Gaussian";
            this.mainWindow = mainWindow;
        }
        private void SingleGaussianOnClick(object sender, RoutedEventArgs e)
        {
            this.fit = "Single";
            FitMenu.Header = "Single Gaussian";
        }
        private void DoubleGaussianOnClick(object sender, RoutedEventArgs e)
        {
            this.fit = "Double";
            FitMenu.Header = "Double Gaussian";
        }

        private void TripleGaussianOnClick(object sender, RoutedEventArgs e)
        {
            this.fit = "Triple";
            FitMenu.Header = "Triple Gaussian";
        }
        private void TetraGaussianOnClick(object sender, RoutedEventArgs e)
        {
            this.fit = "Tetra";
            FitMenu.Header = "Tetra Gaussian";
        }
        private void PentaGaussianOnClick(object sender, RoutedEventArgs e)
        {
            this.fit = "Penta";
            FitMenu.Header = "Penta Gaussian";
        }
        private void HexaGaussianOnClick(object sender, RoutedEventArgs e)
        {
            this.fit = "Hexa";
            FitMenu.Header = "Hexa Gaussian";
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.min = minimum.Text;
                this.max = maximum.Text;
                double min_double = Convert.ToDouble(min);
                double max_double = Convert.ToDouble(max);
                this.fitparams = new FitParams(fit, min_double, max_double);
                app.model.gaussianParamsList = new List<GaussianParams>(); //clear gaussian
                mainWindow.FitConditions();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Specify a range.");
            }

        }
    }
}
