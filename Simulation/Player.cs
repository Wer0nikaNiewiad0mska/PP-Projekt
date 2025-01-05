using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class Player : Creature
{
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

    public void InteractField(BigMap map)
    {
        var adjacentPoints = new[]
        {
        new Point(Position.X, Position.Y + 1),
        new Point(Position.X, Position.Y - 1),
        new Point(Position.X - 1, Position.Y),
        new Point(Position.X + 1, Position.Y)
    };

        foreach (var point in adjacentPoints)
        {
            Console.WriteLine($"Sprawdzanie pola: {point}"); // Debugowanie

            if (!map.TryGetField(point, out var objectsAtPoint))
            {
                Console.WriteLine($"Brak obiektów na pozycji {point}");
                continue;
            }

            var unlockedField = objectsAtPoint.OfType<UnlockedField>().FirstOrDefault();
            if (unlockedField != null)
            {
                Console.WriteLine($"Znaleziono pole wymagające klucza o ID: {unlockedField.KeyId} na pozycji {point}");

                if (unlockedField.BlockedStatus && _keys.Contains(unlockedField.KeyId))
                {
                    // Odblokowanie pola
                    unlockedField.BlockedStatus = false;
                    Console.WriteLine($"Pole {point} zostało odblokowane!");

                    // Usuń pole z listy `objectsAtPoint` (bez modyfikacji `Map.Remove`)
                    objectsAtPoint.Remove(unlockedField);
                    Console.WriteLine($"Pole {point} zostało usunięte z listy obiektów.");
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
        var adjacentPoints = new[]
        {
        new Point(Position.X, Position.Y + 1),
        new Point(Position.X, Position.Y - 1),
        new Point(Position.X - 1, Position.Y),
        new Point(Position.X + 1, Position.Y)
    };

        foreach (var point in adjacentPoints)
        {
            Console.WriteLine($"Sprawdzanie pola: {point}"); // Debugowanie

            if (!map.TryGetField(point, out var objectsAtPoint))
            {
                Console.WriteLine($"Brak obiektów na pozycji {point}");
                continue;
            }

            Console.WriteLine($"Obiekty na pozycji {point}: {string.Join(", ", objectsAtPoint.Select(o => o.GetType().Name))}");

            // Znajdź klucz na polu
            var key = objectsAtPoint.OfType<Key>().FirstOrDefault();
            if (key != null)
            {
                Console.WriteLine($"Znaleziono klucz o ID: {key.KeyId} na pozycji {point}");

                // Dodaj klucz do zbioru gracza
                if (_keys.Add(key.KeyId))
                {
                    // Usuń klucz z mapy
                    map.Remove(key, point);

                    // Usuń klucz z lokalnej listy obiektów
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

    // Metoda pomocnicza do uzyskania sąsiednich punktów
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
