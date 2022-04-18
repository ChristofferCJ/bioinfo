using System;
using project4.Implementations;
namespace project4
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree1Path = "tree1.new";
            var tree2Path = "tree2.new";

            var tree1 = NewickParser.FromFile(tree1Path);
            var tree2 = NewickParser.FromFile(tree2Path);
            var day = new Day();
            var res = day.RFDistance(tree1, tree2);
            Console.WriteLine($"RFDistance: {res}");

            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
    }
}
