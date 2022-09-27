using Raman.CSVReading;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Raman.Fitting
{
    class RangeConvert
    {
        public double[] pixels;
        public double[] intensity;
        public double[] ramanShift;

        public RangeConvert(CsvData csvData, FitParams fitparams)
        {
            List<double> temp_pixels = new List<double>();
            List<double> temp_intensity = new List<double>();
            List<double> temp_ramanShift = new List<double>();

            for (int i = 0; i < csvData.Pixels.Count; i++)
            {
                if (Double.Parse(csvData.RamanShift[i]) > fitparams.Min & Double.Parse(csvData.RamanShift[i]) < fitparams.Max)
                {
                    temp_pixels.Add(Double.Parse(csvData.Pixels[i]));
                    temp_intensity.Add(Double.Parse(csvData.Intensity[i]));
                    temp_ramanShift.Add(Double.Parse(csvData.RamanShift[i]));
                }
            }

            double[] Pixels = new double[temp_pixels.Count];
            double[] Intensity = new double[temp_intensity.Count];
            double[] RamanShift = new double[temp_ramanShift.Count];
            for (int a = 0; a < temp_pixels.Count; a++)
            {
                Pixels[a] = temp_pixels[a];
                Intensity[a] = temp_intensity[a];
                RamanShift[a] = temp_ramanShift[a];
            }

            this.pixels = Pixels;
            this.intensity = Intensity;
            this.ramanShift = RamanShift;
        }
    }
}
