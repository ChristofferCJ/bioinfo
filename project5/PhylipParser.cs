using System.Collections.Generic;
using System.IO;
using System.Globalization;

static class PhylipParser
{
    public static (List<string>, List<List<double>>) FromFile(string filePath)
    {
        var lines = File.ReadAllLines("PhylipFiles/" + filePath);
        var lineCount = int.Parse(lines[0].Trim());

        List<string> names = new();
        List<List<double>> distanceMatrix = new();
        for (int i = 1; i <= lineCount; i++)
        {
            List<double> row = new();
            var line = lines[i];
            var lst = line.Split(' ');
            var name = lst[0];
            names.Add(name);
            for (int j = 1; j <= lineCount; j++)
            {
                var entry = lst[j];
                row.Add(double.Parse(entry.Trim(), CultureInfo.InvariantCulture.NumberFormat));
            }
            distanceMatrix.Add(row);
        }
        return (names, distanceMatrix);
    }

    public static List<List<float>> Test()
    {
        var a = new List<float> {0.00f, 0.23f, 0.16f, 0.20f, 0.17f};
        var b = new List<float> {0.23f, 0.00f, 0.23f, 0.17f, 0.24f};
        var c = new List<float> {0.16f, 0.23f, 0.00f, 0.20f, 0.11f};
        var d = new List<float> {0.20f, 0.17f, 0.20f, 0.00f, 0.21f};
        var e = new List<float> {0.17f, 0.24f, 0.11f, 0.21f, 0.00f};

        var res = new List<List<float>>();
        res.Add(a);
        res.Add(b);
        res.Add(c);
        res.Add(d);
        res.Add(e);

        return res;
    }
}