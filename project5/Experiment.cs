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
            var (names, distanceMatrix) = PhylipParser.FromFile(fileWithoutDirectory);
            var saitouNei = new SaitouNei(names, distanceMatrix);
            var logRunningTime = false;
            timer.Start();
            var tree = saitouNei.ToTree(logRunningTime);
            timer.Stop();
            var outputFileName = fileWithoutDirectory.Replace(".phy", ".new");
            NewickFormatter.ToFile(outputFileName, tree);
            var timeInMs = timer.ElapsedMilliseconds;
            System.Console.WriteLine($"Result for {fileWithoutDirectory}: {timeInMs} ms");
            timer.Reset();
        }
    }
}