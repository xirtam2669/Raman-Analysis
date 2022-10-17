﻿using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using Raman;
using System.Windows;
using System.Windows.Navigation;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using ScottPlot;
using System.Threading;
using System.Windows.Automation.Peers;
using System.Runtime.CompilerServices;

namespace Raman.Fitting
{
    public class CurveFit
    {
        //σ == width
        //μ == center

        public double[] pixels;

        public double[] intensity;

        public double[] ramanShift;

        public LinearParams baslineInitalConditions;

        public GaussianParams gaussianInitialConditions;

        public LinearParams baselineFit;

        public List<GaussianParams> gaussianFit; //was static

        public FitParams fitparams;

        public double[] fitOutput;

        public double rmsErrorThreshold = 10000;

        public string error;

        public double[] y_test = new double[35];

        public CurveFit(double[] Pixels, double[] Intensity, double[] ramanshift, FitParams fitparams)
        {
            this.pixels = Pixels;
            this.intensity = Intensity;
            this.ramanShift = ramanshift;
            this.fitparams = fitparams;
            gaussianFit = new List<GaussianParams>();
        }

        public static double[] Fit(double[] x, double[] raw_y, LinearParams baselineInitialConditions, List<GaussianParams> gaussianInitialConditionsList)
        {
            double epsx = 0.000001;
            int maxits = 0;
            int info;
            alglib.lsfitstate state;
            alglib.lsfitreport rep;
            double diffstep = 0.0001;

            double[,] xx = new double[x.Length, 1];
            for (int i = 0; i < x.Length; i++)
            {
                xx[i, 0] = x[i];
            }
            double[] c = GenCoefficientArrayFromCompositeParams(baselineInitialConditions, gaussianInitialConditionsList);

            alglib.lsfitcreatef(xx, raw_y, c, diffstep, out state);
            alglib.lsfitsetcond(state, epsx, maxits);
            alglib.lsfitfit(state, FitFunc, null, null);
            alglib.lsfitresults(state, out info, out c, out rep);

            if (info != 2)
            {
                //error
            }
            //fitted coefs
            return c;

        }
        private string RMSError(LinearParams linearParams, List<GaussianParams> gaussianParams, double[]x, double[]y)
        {
            double z, square, sum_squares = 0, mean, root, y_calculated;
            for (int i = 0; i < x.Length; i++)
            {
                y_calculated = Composite(linearParams, gaussianParams, x[i]);
                z = y_calculated - y[i];
                square = z * z;
                sum_squares += square;
            }

            mean = sum_squares / x.Length;
            root = Math.Sqrt(mean);
            if(root < rmsErrorThreshold)
            {
                return root.ToString();
            }
            else
            {
                return "Bad fit.";
            }
            
        }
        
        public string RunFit(LinearParams paramsLinearBaselineInitialConditions, List<GaussianParams> paramsGaussianListInitalCondtions, double[] ramanshift, double[] intensity)
        {
            double[] cFitted = Fit(ramanshift, intensity, paramsLinearBaselineInitialConditions, paramsGaussianListInitalCondtions);
            baselineFit = new LinearParams(cFitted[0], cFitted[1]);

            for (int i = 2; i < cFitted.Length; i += 3)
            {
                gaussianFit.Add(new GaussianParams(cFitted[i], cFitted[i + 1], cFitted[i + 2]));
            }

            this.error = RMSError(baselineFit, gaussianFit, ramanshift, intensity);

            this.fitOutput = GenXYFromComposite(baselineFit, gaussianFit, ramanshift);
            return error;
        }

        public static double Gaussian(GaussianParams p, double x) //was private static
        {
            double numerator = (x - p.Center) * (x - p.Center);
            double denominator = 2 * (p.SD * p.SD);
            double output = p.Amplitude * Math.Exp(-numerator / (denominator));
            return output;
        }

        private static double LinearBaseline(LinearParams p, double x)
        {
            return p.Slope * x + p.Intercept;
        }

        private static double Composite(LinearParams paramsBaseline, List<GaussianParams> paramsaGaussianList, double x)
        {
            double r = LinearBaseline(paramsBaseline, x);

            foreach (GaussianParams paramsGuassian in paramsaGaussianList)
            {
                double gaus = Gaussian(paramsGuassian, x);
                r += gaus;
            }
            return r;
        }

        public static double[] GenCoefficientArrayFromCompositeParams(LinearParams paramsBaseline, List<GaussianParams> paramsGaussianList)
        {
            double[] c = new double[2 + 3 * paramsGaussianList.Count];

            c[0] = paramsBaseline.Slope;
            c[1] = paramsBaseline.Intercept;
            int count = 0;

            for (int i = 2; i < c.Length; i = i + 3)
            {
                
                try
                {
                    c[i] = paramsGaussianList[count].Amplitude;
                    c[i + 1] = paramsGaussianList[count].Center;
                    c[i + 2] = paramsGaussianList[count].SD;
                    count++;
                }
                catch
                {
                    return c;
                }
                
            }
            return c;
        }

        public static void FitFunc(double[] c, double[] x, ref double func, object obj)
        {
            LinearParams paramsBaseline = new LinearParams(c[0], c[1]);
            List<GaussianParams> paramsGaussianList = new List<GaussianParams>();

            for (int i = 2; i < c.Length; i += 3)
            {
                paramsGaussianList.Add(new GaussianParams(c[i], c[i + 1], c[i + 2]));
            }
            func = Composite(paramsBaseline, paramsGaussianList, x[0]);
        }
        public double[] GenXYFromComposite(LinearParams paramsBaseline, List<GaussianParams> paramsGaussianList, double[] x)
        {
            double[] y = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = Composite(paramsBaseline, paramsGaussianList, x[i]);
            }
            return y;
        }
    }
}
