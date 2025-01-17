using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class Follower : Creature
{
    public override char Symbol => 'F'; // Symbol reprezentujący followera

    public Point TriggerPoint { get; } // Punkt, którego odblokowanie aktywuje followera
    private bool _isActivated = false; // Czy follower został aktywowany?

    public Follower(string name, Point triggerPoint) : base(name)
    {
        TriggerPoint = triggerPoint;
    }

    public void FollowPlayer(Point previousPlayerPosition, Player player, Map map)
    {
        if (!_isActivated)
        {
            Console.WriteLine($"{Name} czeka na odblokowanie punktu {TriggerPoint}.");
            return;
        }

        // Jeśli follower już stoi na polu, które gracz właśnie opuścił, nic nie robi
        if (Position == previousPlayerPosition)
        {
            Console.WriteLine($"{Name} już stoi na poprzednim polu gracza.");
            return;
        }

        // Sprawdź, czy pole poprzedniej pozycji gracza jest dostępne
        if (CanMoveTo(map, previousPlayerPosition))
        {
            Console.WriteLine($"{Name} rusza z {Position} na {previousPlayerPosition} za graczem.");
            map.Move(this, Position, previousPlayerPosition);
            Position = previousPlayerPosition; // Aktualizacja pozycji followera
        }
        else
        {
            Console.WriteLine($"{Name} nie może się poruszyć na {previousPlayerPosition}. Pole zablokowane lub zajęte.");
        }
    }

    public void ActivateFollower(Point point)
    {
        if (point == TriggerPoint)
        {
            _isActivated = true;
            Console.WriteLine($"{Name} został aktywowany! Teraz podąża za graczem.");
        }
    }
}
