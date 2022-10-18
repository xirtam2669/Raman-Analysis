using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Raman.Fitting
{
    internal class Ratios
    {
        public List<double> ratios = new List<double>();
        public List<double> areas = new List<double>();
        public Ratios(List<double> areas)
        {
            this.areas = areas;
        }

        public List<double> CalculateRatios()
        {
            areas.Clear();
            if (curveFit.gaussianFit.Count == 1)
            {
                AreaOne.Visibility = Visibility.Visible;
                areas.Add(IntegrateGauss(curveFit.gaussianFit[0]));
                GaussianOneArea.Text = areas[0].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios: None";
            }
            if (curveFit.gaussianFit.Count == 2)
            {
                AreaOne.Visibility = Visibility.Visible;
                AreaTwo.Visibility = Visibility.Visible;
                areas.Add(IntegrateGauss(curveFit.gaussianFit[0]));
                areas.Add(IntegrateGauss(curveFit.gaussianFit[1]));
                GaussianOneArea.Text = areas[0].ToString("0.00");
                GaussianTwoArea.Text = areas[1].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios:";

                if (areas[0] > areas[1])
                {
                    ratios.Add(areas[0] / areas[1]);
                }
                else
                {
                    ratios.Add(areas[1] / areas[0]);
                }
            }
            if (curveFit.gaussianFit.Count == 3)
            {
                AreaOne.Visibility = Visibility.Visible;
                AreaTwo.Visibility = Visibility.Visible;
                AreaThree.Visibility = Visibility.Visible;
                areas.Add(IntegrateGauss(curveFit.gaussianFit[0]));
                areas.Add(IntegrateGauss(curveFit.gaussianFit[1]));
                areas.Add(IntegrateGauss(curveFit.gaussianFit[2]));
                GaussianOneArea.Text = areas[0].ToString("0.00");
                GaussianTwoArea.Text = areas[1].ToString("0.00");
                GaussianThreeArea.Text = areas[2].ToString("0.00");
                Ratios.Visibility = Visibility.Visible;
                RatiosDisplay.Text = "Ratios:";
                if (areas[0] > areas[1])
                {
                    ratios.Add(areas[0] / areas[1]);
                }
                else
                {
                    ratios.Add(areas[1] / areas[0]);
                }
                if (areas[1] > areas[2])
                {
                    ratios.Add(areas[1] / areas[2]);
                }
                else
                {
                    ratios.Add(areas[2] / areas[1]);
                }
                if (areas[3] > areas[1])
                {
                    ratios.Add(areas[3] / areas[1]);
                }
                else
                {
                    ratios.Add(areas[1] / areas[3]);
                }
            }
    }
}
