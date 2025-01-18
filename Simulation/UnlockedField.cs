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
        if (string.IsNullOrWhiteSpace(accessCode))
            throw new ArgumentNullException(nameof(accessCode), "Kod dostępu nie może być pusty lub składać się wyłącznie z białych znaków.");

        Position = position; // Punkt jest strukturą, więc nie wymaga sprawdzania null
        KeyId = keyId > 0 ? keyId : throw new ArgumentException("KeyId musi być większy od 0", nameof(keyId));
        AccessCode = accessCode;
    }

    // Implementacja metody InitMapAndPosition
    public void InitMapAndPosition(Map map, Point point)
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map), "Mapa nie może być null.");

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
        if (inventory == null)
            throw new ArgumentNullException(nameof(inventory), "Ekwipunek nie może być null.");

        // Sprawdzenie, czy gracz ma klucz
        var hasKey = inventory.InventoryRecords.Any(record => record.InventoryItem is Key key && key.KeyId == KeyId);
        if (!hasKey)
        {
            Console.WriteLine($"Nie masz klucza do pola {Position}. Musisz posiadać klucz, aby spróbować odblokować.");
            return;
        }

        // Jeśli gracz ma klucz, sprawdzamy kod dostępu
        if (string.Equals(code, AccessCode, StringComparison.Ordinal))
        {
            SetBlockedStatus(false);
            Console.WriteLine($"Pole {Position} zostało odblokowane.");
        }
        else
        {
            Console.WriteLine($"Nieprawidłowy kod dostępu dla pola {Position}.");
        }
    }

    public void SetBlockedStatus(bool status)
    {
        if (BlockedStatus != status)
        {
            BlockedStatus = status;
            Console.WriteLine($"Pole na pozycji {Position} zostało {(status ? "zablokowane" : "odblokowane")}.");
        }
    }

    public override string ToString()
    {
        return $"UnlockedField[Position={Position}, KeyId={KeyId}, BlockedStatus={BlockedStatus}]";
    }
}
