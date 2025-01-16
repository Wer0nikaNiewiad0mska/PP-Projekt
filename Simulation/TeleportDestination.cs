using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class TeleportDestination
{
    public Map Map { get; }
    public Point Position { get; }

    public TeleportDestination(Map map, Point position)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Position = position;
    }
}