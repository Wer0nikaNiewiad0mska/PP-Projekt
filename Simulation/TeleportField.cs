using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Maps;

namespace Simulation;

public class TeleportField : IMappable
{
    public Point Position { get; private set; }
    public char Symbol => 'T'; // Symbol teleportu
    public string TargetMapName { get; }
    public Point TargetPosition { get; }

    public TeleportField(Point position, string targetMapName, Point targetPosition)
    {
        Position = position;
        TargetMapName = targetMapName ?? throw new ArgumentNullException(nameof(targetMapName));
        TargetPosition = targetPosition;
    }

    public void InitMapAndPosition(Map map, Point point)
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map));
        if (!map.Exist(point))
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie istnieje na mapie.");

        Position = point;
        map.Add(this, point);
    }
}