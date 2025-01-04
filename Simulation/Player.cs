﻿using Simulation.Maps;
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

        // Wylicz nową pozycję na podstawie kierunku
        var newPosition = bigMap.Next(Position, direction);

        if (CanMoveTo(bigMap, newPosition)) // Sprawdzamy, czy ruch jest dozwolony
        {
            Console.WriteLine($"Gracz porusza się na pozycję {newPosition}");
            bigMap.Remove(this, Position); // Usuń gracza ze starej pozycji
            Position = newPosition;
            bigMap.Add(this, Position); // Dodaj gracza na nową pozycję
        }
        else
        {
            Console.WriteLine($"Nie można poruszyć się na pozycję {newPosition}");
        }
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

        // Sprawdzenie pól na nowej pozycji
        if (map.TryGetField(newPosition, out var mappableObjects))
        {
            foreach (var obj in mappableObjects)
            {
                // Pole zablokowane przez NPC
                if (obj is Npc)
                {
                    Console.WriteLine($"Ruch zablokowany! Pole zajęte przez NPC.");
                    return false;
                }

                // Pole zablokowane przez pole do odblokowania
                if (obj is UnlockedField field && field.BlockedStatus)
                {
                    Console.WriteLine($"Pole na pozycji {newPosition} jest zablokowane - ruch niemożliwy.");
                    return false;
                }
            }
        }

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
                    unlockedField.BlockedStatus = false;
                    Console.WriteLine($"Pole {point} zostało odblokowane!");
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
        // Znajdź wszystkie sąsiednie pola
        var adjacentPoints = GetAdjacentPoints();

        // Przeszukaj pola w poszukiwaniu klucza
        var keyAtPosition = adjacentPoints
            .Select(point => new { Point = point, Objects = map.TryGetField(point, out var objs) ? objs : null })
            .Where(x => x.Objects != null)
            .SelectMany(x => x.Objects, (x, obj) => new { x.Point, Key = obj as Key })
            .FirstOrDefault(x => x.Key != null);

        if (keyAtPosition != null && keyAtPosition.Key != null)
        {
            var key = keyAtPosition.Key;
            Console.WriteLine($"Znaleziono klucz o ID: {key.KeyId} na pozycji {keyAtPosition.Point}");

            if (_keys.Add(key.KeyId)) // Dodanie klucza do zbioru
            {
                map.Remove(key, keyAtPosition.Point); // Usunięcie klucza z mapy
                Console.WriteLine($"Podniosłeś klucz {key.KeyId} i został on usunięty z mapy!");
            }
            else
            {
                Console.WriteLine($"Masz już ten klucz, nie można go podnieść ponownie.");
            }
        }
        else
        {
            Console.WriteLine("Nie znaleziono klucza w pobliżu.");
        }
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
