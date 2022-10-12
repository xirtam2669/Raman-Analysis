using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman.Fitting
{
    internal class Integrate
    {
        public double Amplitude;
        public double Center;
        public double SD;
        public Integrate(GaussianParams p)
        {
            Amplitude = p.Amplitude;
            Center = p.Center;
            SD = p.SD;
        }

        public double Gaussian(GaussianParams p)
        {
            int x;
            double numerator = (x - p.Center) * (x - p.Center);
            double denominator = 2 * (p.SD * p.SD);
            double output = p.Amplitude * Math.Exp(-numerator / denominator);
            return output;
        }

    }
}
