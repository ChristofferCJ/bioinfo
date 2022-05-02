using System.Collections.Generic;
using System.IO;
using System.Globalization;

static class PhylipParser
{
    public static List<List<float>> FromFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var lineCount = int.Parse(lines[0].Trim());

        List<List<float>> result = new();
        for (int i = 1; i <= lineCount; i++)
        {
            List<float> row = new();
            var line = lines[i];
            var lst = line.Split(' ');
            for (int j = 1; j <= lineCount; j++)
            {
                var entry = lst[j];
                row.Add(float.Parse(entry.Trim(), CultureInfo.InvariantCulture.NumberFormat));
            }
            result.Add(row);
        }
        return result;
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