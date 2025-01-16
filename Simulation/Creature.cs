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

    // Metoda pomocnicza do debugowania
    public override string ToString()
    {
        return $"Creature[Name={Name}, Symbol={Symbol}, Position={Position}, OnMap={Map != null}]";
    }
}