using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Maps;

public class SecondMap : Map
{
    private readonly Dictionary<Point, List<IMappable>> _fields = new();

    public SecondMap(int sizeX, int sizeY) : base(sizeX, sizeY)
    {
        if (sizeX > 1000) throw new ArgumentException(nameof(sizeX), "Wrong width");
        if (sizeY > 1000) throw new ArgumentException(nameof(sizeY), "Wrong height");
    }

    public override Point Next(Point p, Direction d)
    {
        var nextPoint = p.Next(d);
        return Exist(nextPoint) && !IsBlocked(nextPoint) ? nextPoint : p;
    }


    public override bool IsBlocked(Point position)
    {
        if (!_fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<BlockedField>().Any();
    }


    public override bool IsPlayer(Point position)
    {
        if (!_fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<Player>().Any();
    }

    public override bool IsNpc(Point position)
    {
        if (!_fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<Npc>().Any();
    }

    public void AddBlockedField(Point position)
    {
        if (!_fields.ContainsKey(position))
        {
            _fields[position] = new List<IMappable>();
        }
        _fields[position].Add(new BlockedField(position));
    }


    public override bool TryGetField(Point point, out List<IMappable> mappableObjects)
    {
        Console.WriteLine($"Sprawdzanie obiektów na pozycji {point}"); // Debugowanie
        if (_fields.TryGetValue(point, out mappableObjects))
        {
            Console.WriteLine($"Obiekty na pozycji {point}: {string.Join(", ", mappableObjects.Select(o => o.GetType().Name))}");
            return true;
        }

        mappableObjects = null;
        return false;
    }
    
    public override bool IsTeleport(Point position)
    {
        // Sprawdź, czy na pozycji znajduje się teleport
        if (_fields.TryGetValue(position, out var objects))
        {
            return objects.OfType<TeleportField>().Any();
        }
        return false;
    }
}