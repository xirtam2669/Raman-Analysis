using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman
{
    internal class LinearParams
    {
        public double Slope;
        public double Intercept;

        public LinearParams(double slope, double intercept)
        {
            this.Slope = slope;
            this.Intercept = intercept;
        }
    }
}
