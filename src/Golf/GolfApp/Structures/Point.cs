namespace GolfApp.Structures
{
    public enum PointType
    {
        Ball,
        Hole
    }

    public enum PointPosition
    {
        Clockwise,
        Counterclockwise
    }

    public abstract class Point
    {   
        public double X { get; set; }
        public double Y { get; set; }
        public abstract PointType Type { get; }
        public abstract int Value { get; }

        protected Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class Ball : Point
    {
        public Ball(int id, double x, double y) : base(x, y)
        {
            Id = id;
        }

        public int Id { get; }
        public override PointType Type => PointType.Ball;
        public override int Value => 1;
    }

    public class Hole : Point
    {
        public Hole(int id, double x, double y) : base(x, y)
        {
            Id = id;
        }

        public int Id { get; }
        public override PointType Type => PointType.Hole;
        public override int Value => -1;
    }
}