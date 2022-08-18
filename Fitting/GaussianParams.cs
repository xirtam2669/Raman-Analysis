using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman
{
    internal class GaussianParams
    {
        public double Amplitude { get; set; }

        public double σ { get; set; }  

        public double μ { get; set; }

        public GaussianParams(double amplitude, double sigma, double center)
        {
            this.Amplitude = amplitude;
            this.σ = sigma;
            this.μ = center;
        }
    }
}
