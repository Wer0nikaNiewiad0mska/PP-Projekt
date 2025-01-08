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
    public string AccessCode { get; } // Kod dostępu wymagany do odblokowania

    // Status blokady pola
    private bool _blockedStatus = true;
    public bool BlockedStatus
    {
        get => _blockedStatus;
        private set
        {
            if (_blockedStatus != value)
            {
                _blockedStatus = value;
                Console.WriteLine($"Pole na pozycji {Position} zostało {(value ? "zablokowane" : "odblokowane")}.");
            }
        }
    }

    // Konstruktor przyjmujący pozycję, identyfikator klucza i kod dostępu
    public UnlockedField(Point position, int keyId, string accessCode)
    {
        Position = position; // Punkt jest strukturą, więc nie wymaga sprawdzania null
        KeyId = keyId;
        AccessCode = accessCode ?? throw new ArgumentNullException(nameof(accessCode), "Kod dostępu nie może być pusty.");
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

    // Metoda odblokowująca pole przy użyciu kodu dostępu, ale tylko wtedy, gdy mamy klucz
    public void Unlock(string code, InventorySystem inventory)
    {
        // Sprawdzenie, czy gracz ma klucz
        var hasKey = inventory.InventoryRecords.Any(record => record.InventoryItem is Key key && key.KeyId == KeyId);
        if (!hasKey)
        {
            Console.WriteLine($"Nie masz klucza do pola {Position}. Musisz posiadać klucz, aby spróbować odblokować.");
            return;
        }

        // Jeśli gracz ma klucz, sprawdzamy kod dostępu
        if (code == AccessCode)
        {
            BlockedStatus = false;
            Console.WriteLine($"Pole {Position} zostało odblokowane.");
        }
        else
        {
            Console.WriteLine($"Nieprawidłowy kod dostępu dla pola {Position}.");
        }
    }
    public void SetBlockedStatus(bool status)
    {
        BlockedStatus = status;
    }

    public override string ToString()
    {
        return $"UnlockedField[Position={Position}, KeyId={KeyId}, BlockedStatus={BlockedStatus}]";
    }
}