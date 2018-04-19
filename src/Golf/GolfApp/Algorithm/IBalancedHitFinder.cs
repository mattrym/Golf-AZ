using System.Collections.Generic;
using GolfApp.Structures;

namespace GolfApp.Algorithm
{
    public interface IBalancedHitFinder
    {
        Hit FindBalancedHit(IEnumerable<Ball> balls, IEnumerable<Hole> holes);
    }
}