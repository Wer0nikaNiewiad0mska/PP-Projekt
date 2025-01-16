using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public static class MapInitialization
{
    public static BigMap InitializeMap()
    {
        var map = new BigMap(10, 10);
        MapRules.AddBlockedField(map.Fields, new Point(1, 1));
        MapRules.AddPotion(map.Fields, new Point(5, 5), "DoubleMovement");
        return map; // Zwracamy BigMap
    }

    public static Player InitializePlayer(BigMap map)
    {
        var player = new Player("Hero");
        player.InitMapAndPosition(map, new Point(0, 0));
        return player;
    }
}