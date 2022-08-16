using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet;

namespace Raman
{
    internal class CurveFit
    {

        Func<double, double, double, double> gaussian = new Func<double, double, double, double>((σ, μ, x) =>
        {
            return Normal.PDF(μ, σ, x);
        });

        public double curvefit(double[] x, double[] y, Func<double, double, double, double> objective, double initialGuess_σ, double initialGuess_μ)
        {
            Fit.Curve(x, y, objective, initialGuess_σ, initialGuess_μ);
            return 0;
        }
        
    }
}
