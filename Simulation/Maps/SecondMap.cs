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

    public void UnlockField(Point position, int keyId)
    {
        var unlockedField = At(position).OfType<UnlockedField>().FirstOrDefault();
        if (unlockedField != null)
        {
            Console.WriteLine($"Próba odblokowania pola na pozycji {position} przy użyciu klucza {keyId}");
            if (unlockedField.KeyId == keyId && unlockedField.BlockedStatus)
            {
                unlockedField.SetBlockedStatus(false);
                Console.WriteLine($"Pole na pozycji {position} zostało odblokowane!");
            }
            else
            {
                Console.WriteLine($"Nie można odblokować pola. Nieprawidłowy klucz lub pole jest już odblokowane.");
            }
        }
        else
        {
            Console.WriteLine($"Brak pola do odblokowania na pozycji {position}.");
        }
    }

    public override bool IsBlocked(Point position)
    {
        if (!_fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<BlockedField>().Any();
    }

    public override bool IsUnlockable(Point position)
    {
        if (!_fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<UnlockedField>().Any(f => f.BlockedStatus);
    }

    public override bool IsKey(Point position)
    {
        if (!_fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<Key>().Any();
    }

    public override bool IsPotion(Point position)
    {
        if (!_fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<Potions>().Any();
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

    public void AddUnlockedField(Point position, int keyId, string accessCode)
    {
        if (!_fields.ContainsKey(position))
        {
            _fields[position] = new List<IMappable>();
        }
        _fields[position].Add(new UnlockedField(position, keyId, accessCode));
    }

    public void AddKey(Point position, int keyId)
    {
        if (!_fields.ContainsKey(position))
        {
            _fields[position] = new List<IMappable>();
        }

        var key = new Key(position, keyId);
        _fields[position].Add(key);
        Console.WriteLine($"Klucz o ID {keyId} został dodany na pozycję {position}.");
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
    public void AddPotion(Point position, string effect)
    {
        if (!_fields.ContainsKey(position))
        {
            _fields[position] = new List<IMappable>();
        }

        var potion = new Potions(position, effect);
        _fields[position].Add(potion);
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