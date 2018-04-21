using System;
using System.Collections.Generic;
using System.Linq;
using GolfApp.Structures;

namespace GolfApp.Algorithm.Impl
{
    public class PlanarMatchingFinderImpl : IPlanarMatchingFinder
    {
        private readonly IBalancedHitFinder _balancedHitFinder;

        public PlanarMatchingFinderImpl(IBalancedHitFinder balancedHitFinder)
        {
            this._balancedHitFinder = balancedHitFinder;
        }

        public Matching FindPlanarMatching(ICollection<Ball> balls, ICollection<Hole> holes)
        {
            if (balls.Count != holes.Count)
                throw new ArgumentException();

            if (balls.Count == 0)
                return Matching.Empty;
            if (balls.Count == 1)
                return new Matching(new Hit(balls.First(), holes.First()));

            var balancedHit = _balancedHitFinder.FindBalancedHit(balls, holes);

            var ballsClockwise = new List<Ball>();
            var ballsCounterclockwise = new List<Ball>();
            var holesClockwise = new List<Hole>();
            var holesCounterclockwise = new List<Hole>();

            foreach (var ball in balls)
                if (ball.IsClockwise(balancedHit))
                    ballsClockwise.Add(ball);
                else
                    ballsCounterclockwise.Add(ball);
            foreach (var hole in holes)
                if (hole.IsClockwise(balancedHit))
                    holesClockwise.Add(hole);
                else
                    holesCounterclockwise.Add(hole);

            return FindPlanarMatching(ballsClockwise, holesClockwise)
                   + balancedHit
                   + FindPlanarMatching(ballsCounterclockwise, holesCounterclockwise);
        }
    }
}