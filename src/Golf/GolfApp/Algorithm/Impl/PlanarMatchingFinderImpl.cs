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
            balls.Remove(balancedHit.Ball);
            holes.Remove(balancedHit.Hole);

            CompartmentalizePoint(balls, holes, balancedHit, 
                out var ballsClockwise, out var ballsCounterclockwise, 
                out var holesClockwise, out var holesCounterclockwise);

            return FindPlanarMatching(ballsClockwise, holesClockwise)
                   + balancedHit
                   + FindPlanarMatching(ballsCounterclockwise, holesCounterclockwise);
        }

        private static void CompartmentalizePoint(ICollection<Ball> balls, ICollection<Hole> holes, Hit balancedHit,
            out List<Ball> ballsClockwise, out List<Ball> ballsCounterclockwise, 
            out List<Hole> holesClockwise, out List<Hole> holesCounterclockwise)
        {
            ballsClockwise = new List<Ball>();
            ballsCounterclockwise = new List<Ball>();
            holesClockwise = new List<Hole>();
            holesCounterclockwise = new List<Hole>();

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
        }
    }
}