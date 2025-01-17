using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public abstract class Creature : IMappable
{
    private string _name = "Unknown";

    // Nazwa istoty z walidacją
    public string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Nazwa nie może być pusta, bądź składająca się wyłącznie ze spacji.", nameof(value));

            _name = Validator.Shortener(value, 3, 25, '#');
        }
    }

    // Symbol istoty (może być nadpisany przez klasy dziedziczące)
    public virtual char Symbol { get; protected set; } = 'C';

    // Mapa, na której znajduje się istota
    public Map? Map { get; private set; }

    // Pozycja istoty na mapie
    public Point Position { get; protected set; }

    // Konstruktor wymagający nazwy
    public Creature(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    // Inicjalizacja istoty na mapie
    public void InitMapAndPosition(Map map, Point position)
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map), "Mapa nie może być pusta.");

        // Usuń gracza z obecnej mapy, jeśli jest ustawiony
        if (Map != null)
        {
            Console.WriteLine($"Gracz {Name} jest już na mapie. Usuwam z pozycji {Position}.");
            Map.Remove(this, Position);
        }

        if (!map.Exist(position))
            throw new ArgumentOutOfRangeException(nameof(position), "Pozycja nie znajduje się na mapie.");

        Map = map;
        Position = position;
        map.Add(this, position);

        Console.WriteLine($"{Name} został umieszczony na mapie na pozycji {position}.");
    }

    // Usunięcie istoty z mapy
    public void RemoveFromMap()
    {
        if (Map == null)
            throw new InvalidOperationException("Ta postać nie znajduje się na mapie.");

        Map.Remove(this, Position);
        Console.WriteLine($"{Name} został usunięty z pozycji {Position}.");
        Map = null;
    }

    public bool CanMoveTo(Map map, Point newPosition)
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
    // Metoda pomocnicza do debugowania
    public override string ToString()
    {
        return $"Creature[Name={Name}, Symbol={Symbol}, Position={Position}, OnMap={Map != null}]";
    }
}