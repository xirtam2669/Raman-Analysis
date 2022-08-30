using CsvHelper;
using CsvHelper.Configuration;
using Raman.CSVReading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Raman;

class ReadCSV
{
    public CsvData csvData;
    public void Read(string path)
    {
        List<string> Pixels = new List<string>();
        List<string> RamanShift = new List<string>();
        List<string> y_coords = new List<string>();
        List<string> A = new List<string>();

        List<CSV> data = ReadCsv(path);

        foreach (CSV item in data)
        {
            string x = item.D;
            string y = item.H;

            if (String.IsNullOrWhiteSpace(item.D) || x.Contains("Raman Shift"))
            {
                continue;
            }

            else
            {
                if (RamanShift.Count == y_coords.Count)
                {
                    RamanShift.Add(item.D);
                    Pixels.Add(item.A);
                    y_coords.Add(item.H);

                    
                }

                this.csvData = new CsvData(Pixels, RamanShift, y_coords);
            }

        }

    }

    static List<CSV> ReadCsv(string csvFileName)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            IgnoreBlankLines = true,
            MissingFieldFound = null
        };
        using (var reader = new StreamReader(csvFileName))
        using (var csv = new CsvReader(reader, config))
        {  
                var records = csv.GetRecords<CSV>();
                return records.ToList();
        }

    }
}