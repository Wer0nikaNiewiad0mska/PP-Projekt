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
        if (!fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<Key>().Any();
    }

    // Sprawdzenie, czy na danej pozycji znajduje się eliksir
    public static bool IsPotion(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        if (!fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<Potions>().Any();
    }

    // Sprawdzenie, czy na danej pozycji znajduje się pole zablokowane
    public static bool IsBlocked(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        if (!fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<BlockedField>().Any();
    }

    // Sprawdzenie, czy na danej pozycji znajduje się pole odblokowywalne
    public static bool IsUnlockable(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        if (!fields.TryGetValue(position, out var mappableObjects)) return false;
        return mappableObjects.OfType<UnlockedField>().Any(f => f.BlockedStatus);
    }

    // Próba odblokowania pola
    public static void UnlockField(Dictionary<Point, List<IMappable>> fields, Point position, int keyId)
    {
        if (!fields.TryGetValue(position, out var mappableObjects)) return;

        var unlockedField = mappableObjects.OfType<UnlockedField>().FirstOrDefault();
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

    // Dodanie pola zablokowanego
    public static void AddBlockedField(Dictionary<Point, List<IMappable>> fields, Point position)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }
        fields[position].Add(new BlockedField(position));
    }

    // Dodanie pola odblokowywalnego
    public static void AddUnlockedField(Dictionary<Point, List<IMappable>> fields, Point position, int keyId, string accessCode)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }
        fields[position].Add(new UnlockedField(position, keyId, accessCode));
    }

    // Dodanie klucza
    public static void AddKey(Dictionary<Point, List<IMappable>> fields, Point position, int keyId)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }

        var key = new Key(position, keyId);
        fields[position].Add(key);
        Console.WriteLine($"Klucz o ID {keyId} został dodany na pozycję {position}.");
    }

    // Dodanie eliksiru
    public static void AddPotion(Dictionary<Point, List<IMappable>> fields, Point position, string effect)
    {
        if (!fields.ContainsKey(position))
        {
            fields[position] = new List<IMappable>();
        }

        var potion = new Potions(position, effect);
        fields[position].Add(potion);
    }

    // Sprawdzenie obiektów na danej pozycji
    public static bool TryGetField(Dictionary<Point, List<IMappable>> fields, Point point, out List<IMappable> mappableObjects)
    {
        Console.WriteLine($"Sprawdzanie obiektów na pozycji {point}"); // Debugowanie
        if (fields.TryGetValue(point, out mappableObjects))
        {
            Console.WriteLine($"Obiekty na pozycji {point}: {string.Join(", ", mappableObjects.Select(o => o.GetType().Name))}");
            return true;
        }

        mappableObjects = null;
        return false;
    } 
}
