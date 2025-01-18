 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Maps;

public static class MapRules
{
    public static bool IsKey(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        return fields.TryGetValue(position, out var mappableObjects) && mappableObjects.OfType<Key>().Any();
    }

    public static bool IsPotion(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        return fields.TryGetValue(position, out var mappableObjects) && mappableObjects.OfType<Potions>().Any();
    }

    public static bool IsBlocked(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        return fields.TryGetValue(position, out var mappableObjects) &&
               (mappableObjects.OfType<BlockedField>().Any() ||
                mappableObjects.OfType<UnlockedField>().Any(f => f.BlockedStatus));
    }

    public static bool IsNpc(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        return fields.TryGetValue(position, out var mappableObjects) && mappableObjects.OfType<Npc>().Any();
    }

    public static bool IsUnlockable(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        return fields.TryGetValue(position, out var mappableObjects) && mappableObjects.OfType<UnlockedField>().Any(f => f.BlockedStatus);
    }

    public static bool UnlockField(Dictionary<Point, List<IMappable>> fields, Point position, int keyId)
    {
        if (!fields.TryGetValue(position, out var mappableObjects)) return false;

        var unlockedField = mappableObjects.OfType<UnlockedField>().FirstOrDefault();
        if (unlockedField != null && unlockedField.KeyId == keyId && unlockedField.BlockedStatus)
        {
            unlockedField.SetBlockedStatus(false);
            Console.WriteLine($"Field at {position} has been unlocked with key {keyId}.");
            return true;
        }

        return false;
    }

    public static void AddBlockedField(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }
        fields[position].Add(new BlockedField(position));
    }

    public static void AddUnlockedField(Dictionary<Point, List<IMappable>> fields, Point position, int keyId, string accessCode)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }
        fields[position].Add(new UnlockedField(position, keyId, accessCode));
    }

    public static void AddKey(Dictionary<Point, List<IMappable>> fields, Point position, int keyId)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }

        var key = new Key(position, keyId);
        fields[position].Add(key);
        Console.WriteLine($"Key with ID {keyId} added at {position}.");
    }

    public static void AddPotion(Dictionary<Point, List<IMappable>> fields, Point position, string effect)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }

        var potion = new Potions(position, effect);
        fields[position].Add(potion);
        Console.WriteLine($"Potion with effect '{effect}' added at {position}.");
    }

    public static bool TryGetField(Dictionary<Point, List<IMappable>> fields, Point point, out List<IMappable> mappableObjects)
    {
        if (fields.TryGetValue(point, out mappableObjects))
        {
            return true;
        }

        mappableObjects = null;
        return false;
    }
}
