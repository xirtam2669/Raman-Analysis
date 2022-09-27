using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman.Fitting
{
    internal class DoubleGuassianParams
    {
        public double Amplitude_one { get; set; }
        public double μ_one { get; set; }
        public double σ_one { get; set; }
        public double Amplitude_two { get; set; }
        public double μ_two { get; set; }
        public double σ_two { get; set; }

        public DoubleGuassianParams(double amplitude_one, double μ_one, double σ_one, double amplitude_two, double μ_two, double σ_two)
        {
            this.Amplitude_one = amplitude_one;
            this.μ_one = μ_one;
            this.σ_one = σ_one;
            this.Amplitude_two = amplitude_two;
            this.μ_two = μ_two;
            this.σ_two = σ_two;
        }
    }
}
