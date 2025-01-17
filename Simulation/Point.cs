using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public readonly struct Point
{
    public readonly int X, Y;
    public Point(int x, int y) => (X, Y) = (x, y);
    public override string ToString() => $"({X}, {Y})";

    public Point Next(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Point(X, Y + 1);
            case Direction.Down:
                return new Point(X, Y - 1);
            case Direction.Left:
                return new Point(X - 1, Y);
            case Direction.Right:
                return new Point(X + 1, Y);
        }
        return default;
    }
    public override bool Equals(object obj) => obj is Point other && X == other.X && Y == other.Y;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static bool operator !=(Point left, Point right) => !(left == right);
}
