using System.Collections.Generic;

namespace GolfApp.Structures
{
    public class Task
    {
        public List<Ball> Balls { get; }
        public List<Hole> Holes { get; }

        public Task(List<Ball> balls, List<Hole> holes)
        {
            Balls = balls;
            Holes = holes;
        }
    }
}