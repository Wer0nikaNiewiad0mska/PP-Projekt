using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class Rectangle
{
    public readonly int X1; //lewydolny
    public readonly int Y1; //lewydolny
    public readonly int X2; //prawygórny
    public readonly int Y2; //prawygórny


    public Rectangle(int x1, int y1, int x2, int y2)
    {
        if (x1 == x2 || y1 == y2)
        {
            throw new ArgumentException("nie chcemy \"chudych\" prostokątów");
        }

        if (x1 > x2)
        {
            (x1, x2) = (x2, x1);
        }
        if (y1 > y2)
        {
            (y1, y2) = (y2, y1);
        }

        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;

    }

    public Rectangle(Point point1, Point point2)
        : this(point1.X, point1.Y, point2.X, point2.Y)
    { }

    public bool Contains(Point point)
    {
        if (point.X >= X1 && point.X <= X2 && point.Y >= Y1 && point.Y <= Y2) { return true; }
        return false;
    }

    public override string ToString()
    {
        return $"({X1},{Y1}):({X2},{Y2})";
    }

}

