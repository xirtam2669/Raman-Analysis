using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using Raman;

namespace Raman.Fitting
{
    internal class CurveFit
    {
        //σ == width
        //μ == center

        private double[] x_pixel = new double[2048];
        private double[] y_pixel = new double[2048];

        private double peakRange { get; set; }

        public LinearParams baslineInitalConditions;

        public GaussianParams gaussianInitialConditions;

        public LinearParams baselineFit;

        public GaussianParams gaussianFit;

        public FitParams fitparams;

        public double rmsErrorThreshold = 0;
        
        public CurveFit()
        {
            for (int i = 0; i < 2048; i++)
            {
                x_pixel[i] = (double)i;
            }
        }

        public double Fit(double[] y, FitParams fitparams, LinearParams baselineInitalConditions, GaussianParams gaussianInitialConditions)
        {
            double rmsError = 0;

            this.baselineFit = baselineInitalConditions;
            this.gaussianFit = gaussianInitialConditions;
            this.fitparams = fitparams;       

            //Fit curve takes arguments (y, x, composite function, inital conditions, error, max iterations)
            (double slope, double intercept, double amplitude, double μ, double σ) = MathNet.Numerics.Fit.Curve(x_pixel,
                                                                                                                y,
                                                                                                                (slope,
                                                                                                                intercept,
                                                                                                                amplitude,
                                                                                                                σ,
                                                                                                                μ,
                                                                                                                x) => Composite(slope, intercept, amplitude, μ, σ, x),
                                                                                                                baslineInitalConditions.Slope,
                                                                                                                baslineInitalConditions.Intercept,
                                                                                                                gaussianInitialConditions.Amplitude,
                                                                                                                gaussianInitialConditions.μ,
                                                                                                                gaussianInitialConditions.σ,
                                                                                                                1E-9,
                                                                                                                2000);

            rmsError = RMSError(slope, intercept, amplitude, μ, σ, x_pixel, y);

            if(rmsError < rmsErrorThreshold)
            {
                this.baselineFit = new LinearParams(slope, intercept);
                this.gaussianFit = new GaussianParams(amplitude, μ, σ);
            }

            return rmsError;
        }

        private double RMSError(double slope, double intercept, double amplitude, double μ, double σ, double[] x, double[] y)
        {
            double z, square, sum_squares = 0, mean, root, y_calculated;
            for (int i = 0; i < x.Length; i++)
            {
                y_calculated = Composite(slope, intercept, amplitude, μ, σ, x[i]);
                z = y_calculated - y[i];
                square = z * z;
                sum_squares += square;
            }

            mean = sum_squares / x.Length;
            root = Math.Sqrt(mean);
            return root;
        }

        private static double Gaussian(double amplitude, double μ, double σ, double x)
        {
            return Normal.PDF(μ, σ, x);
        }

        private static double LinearBaseline(double slope, double intercept, double x)
        {
            return slope * x + intercept;
        }

        private static double Composite(double slope, double intercept, double amplitude, double μ, double σ, double x)
        {
            return LinearBaseline(slope, intercept, x) + Gaussian( amplitude, σ, μ, x);
        }

    }
}
