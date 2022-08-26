using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raman.CSVReading
{
    public class CsvData
    {
        public List<string> Pixels;
        public List<string> RamanShift;
        public List<string> Intensity;

        public CsvData(List<string> pixels, List<string> ramanShift, List<string> intensity)
        {
            this.Pixels = pixels;
            this.RamanShift = ramanShift;
            this.Intensity = intensity;
        }

    }
}
