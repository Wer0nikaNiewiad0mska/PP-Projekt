using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation;
using Simulation.Maps;

namespace Simulation;

public class Potions : ObtainableItem, IMappable
{
    public Point Position { get; private set; } // Pozycja eliksiru na mapie
    public char Symbol => 'E'; // Symbol eliksiru
    public string Effect { get; } // Efekt eliksiru, np. "DiagonalMovement"

    public Potions(Point position, string effect) : base("Potion", 1)
    {
        Position = position;
        Effect = effect ?? throw new ArgumentNullException(nameof(effect), "Efekt nie może być pusty.");
    }

    public void InitMapAndPosition(Map map, Point point)
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map), "Mapa nie może być pusta.");

        if (!map.Exist(point))
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie istnieje na mapie.");

        Position = point;
        map.Add(this, point);
        Console.WriteLine($"Eliksir o efekcie '{Effect}' został dodany na mapę na pozycję {point}.");
    }

    public override string ToString()
    {
        return $"Potions[Effect={Effect}, Position={Position}]";
    }
}
