using CsvHelper;
using CsvHelper.Configuration;
using Raman;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class ReadCSV
{
    public Tuple<List<string>, List<string>> Read(string path)
    {
        List<string> x_coords = new List<string>();
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
                if (x_coords.Count == y_coords.Count)
                {
                    x_coords.Add(item.D);
                    y_coords.Add(item.H);
                }
            }

        }

        return Tuple.Create(x_coords, y_coords);
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