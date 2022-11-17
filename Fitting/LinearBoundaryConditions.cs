using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman.Fitting
{
    public class LinearBoundaryConditions
    {
        public double sloper_upper;
        public double sloper_lower;
        public double intercept_upper;
        public double intercept_lower;

        public LinearBoundaryConditions(double sloper_upper, double sloper_lower, double intercept_upper, double intercept_lower)
        {
            this.sloper_upper = sloper_upper;
            this.sloper_lower = sloper_lower;
            this.intercept_upper = intercept_upper;
            this.intercept_lower = intercept_lower;
        }
    }
}
