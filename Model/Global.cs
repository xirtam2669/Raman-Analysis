using Raman.CSVReading;
using Raman.Fitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman
{
    public class Global
    {
        public CsvData csvData; //stores pixel, raman shift, intensity
        public FitParams? fitParams;
        public LinearParams linearParams;
        public List<GaussianParams> gaussianParamsList = new List<GaussianParams>();
        public RangeConvert range;
        public CurveFit? curveFit;
        public List<double> areas = new List<double>();
        public List<double> ratios = new List<double>();

        public Global()
        {

        }
    }
}
