using System;
using Bio.Phylogenetics;
namespace project4.Implementations
{
    public interface IImplementation
    {
        public int RFDistance(Tree a, Tree b);
    }
}
