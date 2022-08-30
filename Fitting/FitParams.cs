using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman.Fitting
{
    public class FitParams
    {
        public string Fit;
        public double Min;
        public double Max;

        public FitParams(String fit, double min, double max)
        {
            this.Fit = fit;
            this.Min = min;
            this.Max = max;
        }
    }
}
