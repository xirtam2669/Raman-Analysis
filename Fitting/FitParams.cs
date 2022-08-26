using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman.Fitting
{

    public class FitParams
    {
        public int fit { get; set; }

        public double min { get; set; }

        public double max { get; set; }

        public FitParams(int fit, double min, double max)
        {
            this.fit = fit;
            this.min = min;
            this.max = max;
        }
    }
}
