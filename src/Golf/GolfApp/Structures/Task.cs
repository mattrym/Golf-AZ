using System.Collections.Generic;

namespace GolfApp.Structures
{
    public class Task
    {
        public IList<Ball> Balls { get; } = new List<Ball>();
        public IList<Hole> Holes { get; } = new List<Hole>();
        public int Size => Balls.Count;

        public Task()
        {
        }

        public Task(IList<Ball> balls, IList<Hole> holes)
        {
            Balls = balls;
            Holes = holes;
        }
    }
}