using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class BlockedField : IMappable
{
    public Point Position { get; set; } // Pozycja pola na mapie
    public char Symbol { get; } = 'X'; // Symbol blokowanego pola
    public bool IsBlocked { get; private set; } = true;

    public BlockedField(Point position)
    {
        Position = position;
    }

    // Implementacja InitMapAndPosition
    public void InitMapAndPosition(Map map, Point point)
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map), "Mapa nie może być pusta.");

        if (!map.Exist(point))
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie znajduje się na mapie.");

        Position = point;
        map.Add(this, point);
        Console.WriteLine($"Blokowane pole zostało umieszczone na pozycji {point}.");
    }


    // Metoda pomocnicza do debugowania
    public override string ToString()
    {
        return $"BlockedField[Position={Position}, Symbol={Symbol}, IsBlocked={IsBlocked}]";
    }
}