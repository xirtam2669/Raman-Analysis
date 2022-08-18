using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman
{
    internal class LinearParams
    {
        public double Slope {get; set;}
        public double Intercept {get; set;}

        public LinearParams(double slope, double intercept)
        {
            this.Slope = slope;
            this.Intercept = intercept;
        }
    }
}
