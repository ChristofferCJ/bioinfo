using System.IO;
using project4;
using project4.Implementations;
using Bio.Phylogenetics;

public static class Project5Test
{
    public static void Test()
    {
        var fileNames = Directory.GetFiles("Project5Test/SaitouNei");
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = fileNames[i].Replace("Project5Test/SaitouNei/", "");
        }

        foreach(var file in fileNames)
        {
            Tree saitouNeiTree;
            try
            {
                saitouNeiTree = NewickParser.FromPath($"Project5Test/SaitouNei/{file}");
            }
            catch
            {
                saitouNeiTree = null;
            }

            Tree quickTreeTree;
            try
            {
                quickTreeTree = NewickParser.FromPath($"Project5Test/QuickTree/{file}");
            }
            catch
            {
                quickTreeTree = null;
            }

            Tree rapidNJTree;
            try
            {
                rapidNJTree = NewickParser.FromPath($"Project5Test/RapidNJ/{file}");
            }
            catch
            {
                rapidNJTree = null;
            }
            Day day;
            int RFDistance;

            // Test SaitouNei and Quicktree
            if (saitouNeiTree != null && quickTreeTree != null)
            {
                day = new Day();
                RFDistance = day.RFDistance(saitouNeiTree, quickTreeTree);
                System.Console.WriteLine($"SaitouNei and QuickTree, {file}: {RFDistance}");
            }
            
             // Test SaitouNei and RapidNJ
            if (saitouNeiTree != null && rapidNJTree != null)
            {
                day = new Day();
                RFDistance = day.RFDistance(saitouNeiTree, rapidNJTree);
                System.Console.WriteLine($"SaitouNei and RapdNJ, {file}: {RFDistance}");
            }            

             // Test RapidNJ and Quicktree
            if (rapidNJTree != null && quickTreeTree != null)
            {
                day = new Day();
                RFDistance = day.RFDistance(rapidNJTree, quickTreeTree);
                System.Console.WriteLine($"RapidNJ and QuickTree, {file}: {RFDistance}");
            }
        }
    }
}