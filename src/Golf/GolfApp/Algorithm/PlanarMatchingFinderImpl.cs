using System;
using System.Collections.Generic;
using System.Linq;
using GolfApp.Structures;

namespace GolfApp.Algorithm
{
    public class PlanarMatchingFinderImpl : IPlanarMatchingFinder
    {
        private static PointPosition FindPointPosition(Hit hit, Point point)
        {
            var x0 = hit.Ball.X;
            var y0 = hit.Ball.Y;
            var x1 = hit.Hole.X;
            var y1 = hit.Hole.Y;
            var x2 = point.X;
            var y2 = point.Y;

            return (x1 - x0) * (y2 - y0) - (x2 - x0) * (y1 - y0) < 0
                ? PointPosition.Clockwise
                : PointPosition.Counterclockwise;
        }

        private readonly IBalancedHitFinder _balancedHitFinder;

        public PlanarMatchingFinderImpl(IBalancedHitFinder balancedHitFinder)
        {
            this._balancedHitFinder = balancedHitFinder;
        }

        public Matching FindPlanarMatching(ICollection<Ball> balls, ICollection<Hole> holes)
        {
            if (balls.Count != holes.Count)
                throw new ArgumentException();

            var taskSize = balls.Count;
            if (taskSize == 0)
                return Matching.Empty;
            if (taskSize == 1)
                return new Matching(new Hit(balls.First(), holes.First()));

            var balancedHit = _balancedHitFinder.FindBalancedHit(balls, holes);

            var ballsClockwise = new List<Ball>();
            var ballsCounterclockwise = new List<Ball>();
            var holesClockwise = new List<Hole>();
            var holesCounterclockwise = new List<Hole>();

            foreach (var ball in balls)
                if (FindPointPosition(balancedHit, ball) == PointPosition.Clockwise)
                    ballsClockwise.Add(ball);
                else
                    ballsCounterclockwise.Add(ball);
            foreach (var hole in holes)
                if (FindPointPosition(balancedHit, hole) == PointPosition.Clockwise)
                    holesClockwise.Add(hole);
                else
                    holesCounterclockwise.Add(hole);

            return FindPlanarMatching(ballsClockwise, holesClockwise)
                   + balancedHit
                   + FindPlanarMatching(ballsCounterclockwise, holesCounterclockwise);
        }
    }
}