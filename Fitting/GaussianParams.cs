using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman
{
    public class GaussianParams
    {
        public double Amplitude { get; set; }
        public double Center { get; set; }
        public double SD { get; set; }

        public GaussianParams(double amplitude_one, double center_one, double sigma_one)
        {
            this.Amplitude = amplitude_one;
            this.Center = center_one;
            this.SD = sigma_one;
        }
    } 
}
