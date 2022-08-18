using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using ScottPlot;
using MathNet;


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

        private void TestCsvOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Nullable<bool> result = openFileDialog.ShowDialog();



            if (result == true)
            {
                ReadCSV rc = new ReadCSV();
                Tuple<List<string>, List<string>> data = rc.Read(openFileDialog.FileName);

                x.ItemsSource = data.Item1;
                y.ItemsSource = data.Item2;

                string[] array = data.Item1.ToArray();
                string[] array2 = data.Item2.ToArray();

                double[] x_coords = Array.ConvertAll(array, double.Parse);
                double[] y_coords = Array.ConvertAll(array2, double.Parse);

                plot.Plot.AddScatter(x_coords, y_coords);
                plot.Refresh();

            }

        }

        private void x_SelectionChanged()
        {

        }
    }
}