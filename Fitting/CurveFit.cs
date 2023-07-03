using MathNet.Numerics;
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

        public double[] pixels; //X axis

        public double[] intensity; //Y Axis

        public double[] ramanShift; //X axis

        public LinearParams baslineInitalConditions; //

        public GaussianParams gaussianInitialConditions;

        public LinearParams baselineFit;

        public List<GaussianParams> gaussianFit; //was static

        public FitParams fitparams;

        public double[] fitOutput;

        public double rmsErrorThreshold = 500;

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

        public static double[] Fit(double[] x, double[] raw_y, LinearParams baselineInitialConditions, List<GaussianParams> gaussianInitialConditionsList, List<BoundaryConditions> boundaryConditionsList, LinearBoundaryConditions linearBoundaryConditions, bool boundaryFlag)
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
            double[] c = GenCoefficientArrayFromCompositeParams(baselineInitialConditions, gaussianInitialConditionsList); //create c array for holding equation parameters                    

                double[] bndu = GenBndu(boundaryConditionsList, linearBoundaryConditions); //Lower Boundary Conditions
                double[] bndl = GenBndl(boundaryConditionsList, linearBoundaryConditions); //Upper Boundary Conditions

            if (boundaryFlag == true) //curve fit with boundaries
            {
                alglib.lsfitcreatef(xx, raw_y, c, diffstep, out state);
                alglib.lsfitsetcond(state, epsx, maxits);
                alglib.lsfitsetbc(state, bndl, bndu);
                alglib.lsfitfit(state, FitFunc, null, null);
                alglib.lsfitresults(state, out info, out c, out rep);
            }
            else //curve fit without boundaries
            {
                alglib.lsfitcreatef(xx, raw_y, c, diffstep, out state);
                alglib.lsfitsetcond(state, epsx, maxits);
                alglib.lsfitfit(state, FitFunc, null, null);
                alglib.lsfitresults(state, out info, out c, out rep);
            }

            if (info != 2)
            {
                //error
            }
            //fitted coefs
            return c; //return composite params
        }

        private static double[] GenBndu(List<BoundaryConditions> boundaryConditionsList, LinearBoundaryConditions linearBoundaryConditions) //upper boundary conditions
        { 
            int count = 0;
            List<double> bndu_temp = new List<double>();
            for (int i = 0; i < boundaryConditionsList.Count; i++)
            {
                if(i == 0)
                {
                    bndu_temp.Add(linearBoundaryConditions.sloper_upper);
                    bndu_temp.Add(linearBoundaryConditions.intercept_upper);
                }
                bndu_temp.Add(boundaryConditionsList[i].amplitude_upper);
                bndu_temp.Add(boundaryConditionsList[i].center_upper);
                bndu_temp.Add(boundaryConditionsList[i].sd_upper);
            }
            double[] bndu = bndu_temp.ToArray();
            return bndu;
        }

        private static double[] GenBndl(List<BoundaryConditions> boundaryConditionsList, LinearBoundaryConditions linearBoundaryConditions) //lower boundary conditions
        {
            int count = 0;
            List<double> bndu_temp = new List<double>();
            for (int i = 0; i < boundaryConditionsList.Count; i++)
            {
                if (i == 0)
                {
                    bndu_temp.Add(linearBoundaryConditions.sloper_lower);
                    bndu_temp.Add(linearBoundaryConditions.intercept_lower);
                }
                bndu_temp.Add(boundaryConditionsList[i].amplitude_lower);
                bndu_temp.Add(boundaryConditionsList[i].center_lower);
                bndu_temp.Add(boundaryConditionsList[i].sd_lower);
            }
            double[] bndu = bndu_temp.ToArray();
            return bndu;
        }

        private string RMSError(LinearParams linearParams, List<GaussianParams> gaussianParams, double[]x, double[]y)
        {
            double z, square, sum_squares = 0, mean, root, y_calculated;
            for (int i = 0; i < x.Length; i++)
            {
                y_calculated = Composite(linearParams, gaussianParams, x[i]); //Calculate composite function for given x
                z = y_calculated - y[i]; //composite minus actual y to get the error (z)
                square = z * z; //square the error
                sum_squares += square; //add squared error to sum of squared errors
            }

            mean = sum_squares / x.Length; //divide sum of squared errors by the amount of times the function ran.
            root = Math.Sqrt(mean); // square root the mean
            if(root < rmsErrorThreshold) //if the error is below the threshold
            {
                return root.ToString(); //successful fit
            }
            else
            {
                return "Bad fit."; //Error is above threshold
            }
            
        }
        
        public string RunFit(LinearParams paramsLinearBaselineInitialConditions, List<GaussianParams> paramsGaussianListInitalCondtions, List<BoundaryConditions> boundaryConditionsList, LinearBoundaryConditions linearBoundaryConditions, double[] ramanshift, double[] intensity, bool boundaryFlag)
        {
            double[] cFitted = Fit(ramanshift, intensity, paramsLinearBaselineInitialConditions, paramsGaussianListInitalCondtions, boundaryConditionsList, linearBoundaryConditions, boundaryFlag); //cFitted is an array of composite function parameters
            baselineFit = new LinearParams(cFitted[0], cFitted[1]); //linear baseline composite parameters

            for (int i = 2; i < cFitted.Length; i += 3)
            {
                gaussianFit.Add(new GaussianParams(cFitted[i], cFitted[i + 1], cFitted[i + 2])); //pulling gaussian parameters out of cFitted into a gaussianFit object
            }

            this.error = RMSError(baselineFit, gaussianFit, ramanshift, intensity); //calculate squared error 

            this.fitOutput = GenXYFromComposite(baselineFit, gaussianFit, ramanshift); //execute composite function and collect all output for graphing onto screen
            return error; //return the error to the UI
        }

        public static double Gaussian(GaussianParams p, double x) //was private static. This function is the gaussian equation.
        {
            double numerator = (x - p.Center) * (x - p.Center);
            double denominator = 2 * (p.SD * p.SD);
            double output = p.Amplitude * Math.Exp(-numerator / (denominator));
            return output;
        }

        private static double LinearBaseline(LinearParams p, double x) //linear baseline 
        {
            return p.Slope * x + p.Intercept;
        }

        private static double Composite(LinearParams paramsBaseline, List<GaussianParams> paramsaGaussianList, double x) //composite function
        {
            double r = LinearBaseline(paramsBaseline, x); //calculates linear baseline

            foreach (GaussianParams paramsGuassian in paramsaGaussianList)
            {
                double gaus = Gaussian(paramsGuassian, x); //calculate gaussian
                r += gaus; //add linear baseline to gaussian to create composite function
            }
            return r; //return output
        }

        public static double[] GenCoefficientArrayFromCompositeParams(LinearParams paramsBaseline, List<GaussianParams> paramsGaussianList)
        {
            double[] c = new double[2 + 3 * paramsGaussianList.Count]; //array of parameters

            c[0] = paramsBaseline.Slope; //Slope
            c[1] = paramsBaseline.Intercept; //intercept
            int count = 0;

            for (int i = 2; i < c.Length; i = i + 3) //increment by three, as after slope and intercept there are groups of three parameters for each peak
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
            LinearParams paramsBaseline = new LinearParams(c[0], c[1]); //linear baseline
            List<GaussianParams> paramsGaussianList = new List<GaussianParams>(); 

            for (int i = 2; i < c.Length; i += 3) //adding each set of gaussian parameters to C
            {
                paramsGaussianList.Add(new GaussianParams(c[i], c[i + 1], c[i + 2])); 
            }
            func = Composite(paramsBaseline, paramsGaussianList, x[0]); //execute composite function
        }
        public double[] GenXYFromComposite(LinearParams paramsBaseline, List<GaussianParams> paramsGaussianList, double[] x)
        {
            double[] y = new double[x.Length]; //create y array
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = Composite(paramsBaseline, paramsGaussianList, x[i]); //populate y array with composite function output
            }
            return y;
        }
    }
}
