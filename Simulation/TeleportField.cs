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
    public char Symbol => 'T'; // Symbol pola teleportacyjnego
    public string TargetMapName { get; } // Nazwa docelowej mapy
    public Point TargetPosition { get; } // Pozycja na docelowej mapie

    public TeleportField(Point position, string targetMapName, Point targetPosition)
    {
        Position = position;
        TargetMapName = targetMapName;
        TargetPosition = targetPosition;
    }

    public void InitMapAndPosition(Map map, Point point)
    {
        Position = point;
    }
}