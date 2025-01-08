using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class Key : ObtainableItem, IMappable
{
    public Point Position { get; private set; }
    public char Symbol => '*';
    public int KeyId { get; } // Unikalny identyfikator klucza

    public Key(Point position, int keyId) : base("Key", 1)
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
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie znajduje się na mapie.");

        Position = point;
        map.Add(this, point);
        Console.WriteLine($"Klucz o ID {KeyId} został umieszczony na pozycji {point}.");
    }

    public override string ToString()
    {
        return $"Key[ID={KeyId}, Position={Position}, Symbol={Symbol}]";
    }
}