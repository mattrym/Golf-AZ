using System.Collections.Generic;
using GolfApp.Structures;

namespace GolfApp.Algorithm
{
    public interface IPlanarMatchingFinder
    {
        Matching FindPlanarMatching(ICollection<Ball> balls, ICollection<Hole> holes);
    }
}