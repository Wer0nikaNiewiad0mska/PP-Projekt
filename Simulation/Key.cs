using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class Key : IMappable
{
    public Point Position { get; private set; } // Pozycja klucza na mapie
    public char Symbol => '*'; // Symbol klucza
    public int KeyId { get; } // Unikalny identyfikator klucza

    // Konstruktor
    public Key(Point position, int keyId)
    {
        if (keyId <= 0)
            throw new ArgumentException("ID klucza musi być większe od 0", nameof(keyId));

        Position = position;
        KeyId = keyId;
    }

    // Implementacja InitMapAndPosition
    public void InitMapAndPosition(Map map, Point point)
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map), "Mapa nie może być pusta.");
        if (!map.Exist(point))
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie istnieje na mapie.");

        Position = point;
        map.Add(this, point);
        Console.WriteLine($"Klucz o ID {KeyId} został umieszczony na pozycji {point}.");
    }

    // Metoda pomocnicza do debugowania
    public override string ToString()
    {
        return $"Key[ID={KeyId}, Position={Position}, Symbol={Symbol}]";
    }
}
