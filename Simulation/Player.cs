using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation;
using System.Text.Json;

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

        Console.WriteLine($"Gracz: {Name}, obecna pozycja: {Position}, próba ruchu w kierunku: {direction}");

        Point newPosition;

        // Sprawdzenie, czy efekt "DoubleMovement" jest aktywny
        if (_effects.Contains("DoubleMovement"))
        {
            Console.WriteLine("Efekt 'DoubleMovement' aktywny. Ruch podwójny.");
            newPosition = bigMap.Next(Position, direction);
            newPosition = bigMap.Next(newPosition, direction);
            _effects.Remove("DoubleMovement"); // Usuń efekt po użyciu
        }
        else
        {
            Console.WriteLine("Efekt 'DoubleMovement' nieaktywny. Ruch pojedynczy.");
            newPosition = bigMap.Next(Position, direction);
        }
        Console.WriteLine($"Nowa pozycja wyliczona: {newPosition}");

        if (!CanMoveTo(bigMap, newPosition))
        {
            Console.WriteLine($"Nie można poruszyć się na pozycję {newPosition}. Ruch zablokowany.");
            return;
        }

        // Aktualizacja pozycji
        Console.WriteLine($"Poruszam się z {Position} na {newPosition}.");
        bigMap.Remove(this, Position);
        Position = newPosition;
        bigMap.Add(this, Position);
        Console.WriteLine($"Nowa pozycja gracza: {Position}");
    }

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

                // Pole zajęte przez eliksir
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
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (!map.TryGetField(point, out var objectsAtPoint))
                continue;

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
                return;
            }
        }

        Console.WriteLine("Nie znaleziono eliksiru w pobliżu.");
    }

    public void UsePotion(string effect)
    {
        if (string.IsNullOrEmpty(effect))
        {
            Console.WriteLine("Nazwa efektu nie może być pusta.");
            return;
        }

        var potionRecord = Inventory.InventoryRecords
            .FirstOrDefault(record => record.InventoryItem is Potions potion && potion.Effect == effect);

        if (potionRecord == null)
        {
            Console.WriteLine($"Nie posiadasz eliksiru o efekcie: {effect}.");
            return;
        }

        Console.WriteLine($"Aktywacja efektu: {effect}");
        _effects.Add(effect);

        potionRecord.ReduceQuantity(1);
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
            new Point(Position.X + 1, Position.Y),
        };
    }
}