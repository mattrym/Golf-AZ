using System;
using System.Collections.Generic;
using System.Linq;
using GolfApp.Structures;

namespace GolfApp.Algorithm
{
    public class BalancedHitFinderImpl : IBalancedHitFinder
    {
        public Hit FindBalancedHit(IEnumerable<Ball> balls, IEnumerable<Hole> holes)
        {
            var points = AllPoints(balls, holes);
            var minimalPoint = FindMinimalPoint(points);

            MoveOriginToMinimalPoint(points, minimalPoint);
            SortPointsByTangent(points, minimalPoint);

            var balance = 0;
            foreach (var point in points)
            {
                balance += point.Value;
                if (balance == 0)
                    return CreateHit(minimalPoint, point);
            }

            throw new ArgumentException();
        }

        private static List<Point> AllPoints(IEnumerable<Ball> balls, IEnumerable<Hole> holes)
        {
            var points = new List<Point>();
            points.AddRange(balls);
            points.AddRange(holes);

            return points;
        }

        private static Point FindMinimalPoint(List<Point> points)
        {
            var minimalPoint = points.First();
            foreach (var point in points)
            {
                if (point.Y < minimalPoint.Y)
                {
                    minimalPoint = point;
                }
                else if (Math.Abs(point.Y - minimalPoint.Y) < Constants.Eps
                         && point.X < minimalPoint.X)
                {
                    minimalPoint = point;
                }
            }

            return minimalPoint;
        }

        private static void MoveOriginToMinimalPoint(List<Point> points, Point minimalPoint)
        {
            var minimalX = minimalPoint.X;
            var minimalY = minimalPoint.Y;

            foreach (var point in points)
            {
                point.X -= minimalX;
                point.Y -= minimalY;
            }
        }

        private static void SortPointsByTangent(List<Point> points, Point minimalPoint)
        {
            points.Sort((p1, p2) =>
            {
                if (p1.Equals(minimalPoint))
                    return -1;
                if (p2.Equals(minimalPoint))
                    return 1;
                return Math.Sign(p1.Y * p2.X - p2.Y * p1.X);
            });
        }

        private static Hit CreateHit(Point minimalPoint, Point point)
        {
            if (minimalPoint.Type == point.Type)
                throw new ArgumentException();

            var ball = point.Type == PointType.Ball ? point as Ball : minimalPoint as Ball;
            var hole = point.Type == PointType.Hole ? point as Hole : minimalPoint as Hole;
            return new Hit(ball, hole);
        }
    }
}