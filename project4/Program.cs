using System;
using project4.Implementations;
namespace project4
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree1Path = "rapidnj-muscle.new";
            var tree2Path = "rapidnj-clustal-omega.new";

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
