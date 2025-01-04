using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public static class DirectionParser
{
    public static List<Direction> Parse(string input)
    {
        return input.ToUpper()
        .Where(ch => "URDL".Contains(ch))
        .Select(ch => ch switch
        {
            'U' => Direction.Up,
            'R' => Direction.Right,
            'D' => Direction.Down,
            'L' => Direction.Left,
            _ => throw new InvalidOperationException("Invalid direction")
        }).ToList();
    }
}
