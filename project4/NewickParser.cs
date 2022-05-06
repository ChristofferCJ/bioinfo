using System;
using System.IO;
using BioIO = Bio.IO.Newick;
using Bio.Phylogenetics;
namespace project4
{
    public static class NewickParser
    {
        public static Tree FromFile(string path)
        {
            Stream stream;
            try
            {
                stream = File.OpenRead($"Data/{path}");
            }
            catch
            {
                Console.WriteLine($"Error opening file {path}");
                throw;
            }

            var parser = new BioIO.NewickParser();
            return parser.Parse(stream);
        }

         public static Tree FromPath(string path)
        {
            Stream stream;
            try
            {
                stream = File.OpenRead($"{path}");
            }
            catch
            {
                Console.WriteLine($"Error opening file {path}");
                throw;
            }

            var parser = new BioIO.NewickParser();
            return parser.Parse(stream);
        }
    }
}