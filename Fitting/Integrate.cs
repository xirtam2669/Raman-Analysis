using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Raman.Fitting
{
    internal class Integrate
    {
        public double Amplitude;
        public double Center;
        public double SD;
        public double[] x;
        public double[] y;
        public double Slope;
        public Integrate(GaussianParams p)
        {
            Amplitude = p.Amplitude;
            Center = p.Center;
            SD = p.SD;
            //Slope = slope;
            //this.x = x;
            //this.y = y;
        }  

        public double run()
        {
            //double integral = MathNet.Numerics.Integration.SimpsonRule.IntegrateComposite(x => (Amplitude * Math.Exp(-((x - Center) * (x - Center)) / (2 * (SD * SD)))),
            //                                                          Center - (10 * SD / 2), Center + (10 * SD / 2), 100);

            double integral = Amplitude * Math.Abs(SD) * Math.Sqrt(2 * Math.PI);
            
            return integral;
        }
    }
}
