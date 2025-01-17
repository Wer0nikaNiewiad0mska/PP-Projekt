using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Maps;

public class SecondMap : Map
{
    private readonly Dictionary<Point, List<IMappable>> _fields = new();
    public Dictionary<Point, List<IMappable>> Fields => _fields;


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
        return MapRules.IsBlocked(_fields, position);
    }

    public override bool IsUnlockable(Point position)
    {
        return MapRules.IsUnlockable(_fields, position);
    }

    public void UnlockField(Point position, int keyId)
    {
        MapRules.UnlockField(_fields, position, keyId);
    }

    public override bool IsNpc(Point position)
    {
        return MapRules.IsNpc(_fields, position);
    }
    public override bool IsPotion(Point position) => MapRules.IsPotion(_fields, position);

    public override bool IsKey(Point position) => MapRules.IsKey(_fields, position);

    public void AddBlockedField(Point position)
    {
        MapRules.AddBlockedField(_fields, position);
    }

    public void AddUnlockedField(Point position, int keyId, string accessCode)
    {
        MapRules.AddUnlockedField(_fields, position, keyId, accessCode);
    }

    public void AddKey(Point position, int keyId)
    {
        MapRules.AddKey(_fields, position, keyId);
    }

    public void AddPotion(Point position, string effect)
    {
        MapRules.AddPotion(_fields, position, effect);
    }

    public bool TryGetField(Point point, out List<IMappable> mappableObjects)
    {
        return MapRules.TryGetField(_fields, point, out mappableObjects);
    }
}