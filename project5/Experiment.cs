using System.IO;
using System.Diagnostics;
using Implementation;

static class Experiment
{
    public static void Test()
    {
        var phylipFiles = Directory.GetFiles("PhylipFiles");
        foreach(var file in phylipFiles)
        {
            var fileWithoutDirectory = file.Replace("PhylipFiles/", "");
            var timer = new Stopwatch();
            System.Console.WriteLine($"Running test for {fileWithoutDirectory}");
            timer.Start();
            var (names, distanceMatrix) = PhylipParser.FromFile(fileWithoutDirectory);
            var tree = SaitouNei.ToNewickFormat(names, distanceMatrix);
            NewickFormatter.ToFile(fileWithoutDirectory, tree);
            timer.Stop();
            var timeInMs = timer.ElapsedMilliseconds;
            System.Console.WriteLine($"Result for {fileWithoutDirectory}: {timeInMs} ms");
            timer.Reset();
        }
    }
}