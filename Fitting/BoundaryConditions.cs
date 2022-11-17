using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman.Fitting
{
    public class BoundaryConditions
    {
        public double amplitude_upper { get; set; }
        public double amplitude_lower { get; set; }
        public double center_upper { get; set; }
        public double center_lower { get; set; }
        public double sd_upper { get; set; }
        public double sd_lower { get; set; }
        public BoundaryConditions(double Amplitude_upper, double Amplitude_lower, double Center_upper, 
                                  double Center_lower, double SD_upper, double SD_lower) {

            this.amplitude_upper = Amplitude_upper;
            this.amplitude_lower = Amplitude_lower;
            this.center_upper = Center_upper;
            this.center_lower = Center_lower;
            this.sd_upper = SD_upper;
            this.sd_lower = SD_lower;
        }
    }
}
