using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation;

namespace Simulation;

public class Player : Creature
{
    private readonly HashSet<string> _effects = new(); // Lista aktywnych efektów
    public InventorySystem Inventory { get; } = new(); // Ekwipunek gracza
    public override char Symbol => 'P'; // Stały symbol gracza

    private readonly HashSet<int> _keys = new();
    public Player(string name) : base(name) { }

    public void Go(Direction direction)
    {
        if (Map is not BigMap bigMap)
            throw new InvalidOperationException("Map is not set or is not a BigMap.");

        // Wylicz nową pozycję
        var newPosition = bigMap.Next(Position, direction);

        // Sprawdzenie możliwości ruchu za pomocą `CanMoveTo`
        if (!CanMoveTo(bigMap, newPosition))
        {
            Console.WriteLine($"Nie można poruszyć się na pozycję {newPosition}. Ruch zablokowany.");
            return;
        }

        // Ruch jest możliwy - zaktualizuj pozycję
        Console.WriteLine($"Gracz porusza się na pozycję {newPosition}.");
        bigMap.Remove(this, Position); // Usuń gracza ze starej pozycji
        Position = newPosition;
        bigMap.Add(this, Position); // Dodaj gracza na nową pozycję
    }

    public bool HasKey(int keyId) => _keys.Contains(keyId);

    public bool CanMoveTo(BigMap map, Point newPosition)
    {
        // Sprawdzenie, czy nowa pozycja istnieje na mapie
        if (!map.Exist(newPosition))
        {
            Console.WriteLine($"Pozycja {newPosition} nie istnieje na mapie!");
            return false;
        }

        // Pobierz obiekty na nowej pozycji
        if (map.TryGetField(newPosition, out var mappableObjects))
        {
            foreach (var obj in mappableObjects)
            {
                // Pole zajęte przez NPC
                if (obj is Npc npc)
                {
                    Console.WriteLine($"Ruch zablokowany! Pole {newPosition} zajęte przez NPC {npc.Name}.");
                    return false;
                }

                // Pole zajęte przez klucz
                if (obj is Key)
                {
                    Console.WriteLine($"Ruch zablokowany! Pole {newPosition} zajęte przez klucz.");
                    return false;
                }

                // Pole zajęte przez potke
                if (obj is Potions)
                {
                    Console.WriteLine($"Ruch zablokowany! Pole {newPosition} zajęte przez eliksir.");
                    return false;
                }

                // Pole zablokowane przez `UnlockedField`
                if (obj is UnlockedField field && field.BlockedStatus)
                {
                    Console.WriteLine($"Ruch zablokowany! Pole {newPosition} jest zablokowane (Y).");
                    return false;
                }

                // Pole zablokowane przez `BlockedField`
                if (obj is BlockedField)
                {
                    Console.WriteLine($"Ruch zablokowany! Pole {newPosition} jest zablokowane (X).");
                    return false;
                }
            }
        }

        // Jeśli pole jest wolne, ruch jest możliwy
        return true;
    }

    public void InteractField(BigMap map, string accessCode)
    {
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (!map.TryGetField(point, out var objectsAtPoint))
                continue;

            var unlockedField = objectsAtPoint.OfType<UnlockedField>().FirstOrDefault();
            if (unlockedField != null)
            {
                if (unlockedField.BlockedStatus && _keys.Contains(unlockedField.KeyId))
                {
                    if (accessCode == unlockedField.AccessCode)
                    {
                        unlockedField.SetBlockedStatus(false);
                        Console.WriteLine($"Pole {point} zostało odblokowane!");
                    }
                    else
                    {
                        Console.WriteLine($"Nieprawidłowy kod dostępu dla pola {point}.");
                    }
                }
                else
                {
                    Console.WriteLine($"Nie możesz odblokować tego pola. Czy masz odpowiedni klucz?");
                }
                return;
            }
        }

        Console.WriteLine("Nie znaleziono pola do odblokowania w pobliżu.");
    }

    public void InteractKey(BigMap map)
    {
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (!map.TryGetField(point, out var objectsAtPoint))
                continue;

            var key = objectsAtPoint.OfType<Key>().FirstOrDefault();
            if (key != null)
            {
                if (_keys.Add(key.KeyId))
                {
                    map.Remove(key, point);
                    objectsAtPoint.Remove(key);
                    Console.WriteLine($"Podniosłeś klucz {key.KeyId} i został on usunięty z mapy!");
                }
                else
                {
                    Console.WriteLine($"Masz już ten klucz, nie można go podnieść ponownie.");
                }
                return;
            }
        }

        Console.WriteLine("Nie znaleziono klucza w pobliżu.");
    }

    public void InteractPotion(BigMap map)
    {
        // Pobranie sąsiednich punktów
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (!map.TryGetField(point, out var objectsAtPoint))
                continue;

            // Znajdź pierwszy eliksir na polu
            var potion = objectsAtPoint.OfType<Potions>().FirstOrDefault();
            if (potion != null)
            {
                Console.WriteLine($"Znalazłeś eliksir: {potion.Effect}");

                // Dodaj eliksir do ekwipunku
                Inventory.AddItem(potion, 1);

                // Usuń eliksir z mapy
                map.Remove(potion, point);
                objectsAtPoint.Remove(potion);

                Console.WriteLine($"Podniosłeś eliksir o efekcie '{potion.Effect}'!");
                return; // Zbieramy tylko jeden eliksir na raz
            }
        }

        Console.WriteLine("Nie znaleziono eliksiru w pobliżu.");
    }

    public void UsePotion(string effect)
    {
        // Znajdź eliksir w ekwipunku o podanym efekcie
        var potionRecord = Inventory.InventoryRecords.FirstOrDefault(record => record.InventoryItem is Potions potion && potion.Effect == effect);

        if (potionRecord == null)
        {
            Console.WriteLine("Nie posiadasz eliksiru o takim efekcie.");
            return;
        }

        // Aktywuj efekt eliksiru
        Console.WriteLine($"Użyto eliksiru o efekcie: {effect}");
        _effects.Add(effect); // Dodaj efekt do aktywnych efektów
        potionRecord.ReduceQuantity(1); // Zmniejsz ilość eliksirów

        // Usuń eliksir z ekwipunku, jeśli jego ilość spadnie do 0
        if (potionRecord.Quantity <= 0)
        {
            Inventory.InventoryRecords.Remove(potionRecord);
        }
    }

    private IEnumerable<Point> GetAdjacentPoints()
    {
        return new[]
        {
            new Point(Position.X, Position.Y + 1),
            new Point(Position.X, Position.Y - 1),
            new Point(Position.X - 1, Position.Y),
            new Point(Position.X + 1, Position.Y)
        };
    }
}