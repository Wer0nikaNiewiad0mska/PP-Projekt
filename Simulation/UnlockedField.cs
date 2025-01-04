using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class UnlockedField : IMappable
{
    public Point Position { get; set; } // Pozycja pola na mapie
    public char Symbol => BlockedStatus ? 'Y' : ' '; // Symbol zablokowanego/odblokowanego pola
    public int KeyId { get; } // Identyfikator klucza

    // Status blokady pola
    private bool _blockedStatus = true;
    public bool BlockedStatus
    {
        get => _blockedStatus;
        set
        {
            if (_blockedStatus != value)
            {
                _blockedStatus = value;
                Console.WriteLine($"Pole na pozycji {Position} zostało {(value ? "zablokowane" : "odblokowane")}.");
            }
        }
    }

    // Konstruktor przyjmujący pozycję i identyfikator klucza
    public UnlockedField(Point position, int keyId)
    {
        Position = position; // Punkt jest strukturą, więc nie wymaga sprawdzania null
        KeyId = keyId;
    }

    // Implementacja metody InitMapAndPosition
    public void InitMapAndPosition(Map map, Point point)
    {
        if (map == null) throw new ArgumentNullException(nameof(map));
        if (!map.Exist(point))
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie istnieje na mapie.");

        // Ustawienie pozycji i dodanie pola do mapy
        Position = point;
        map.Add(this, point);
        Console.WriteLine($"Pole {this} zostało zainicjalizowane na pozycji {point}.");
    }

    public override string ToString()
    {
        return $"UnlockedField[Position={Position}, KeyId={KeyId}, BlockedStatus={BlockedStatus}]";
    }
}
