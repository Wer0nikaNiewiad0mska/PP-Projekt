using Simulation.Maps;

namespace Simulation;

public class Player : Creature
{
    private readonly HashSet<string> _effects = new(); // Lista aktywnych efektów
    public InventorySystem Inventory { get; } = new(); // Ekwipunek gracza
    public override char Symbol => 'P'; // Stały symbol gracza

    private readonly HashSet<int> _keys = new();
    public Player(string name) : base(name) { }

    public void Go(Direction direction)
    {
        if (Map is not BigMap && Map is not SecondMap)
            throw new InvalidOperationException("Map is not set or is not a valid type (BigMap or SecondMap).");

        Console.WriteLine($"Gracz: {Name}, obecna pozycja: {Position}, próba ruchu w kierunku: {direction}");

        Point newPosition;

        // Sprawdzenie, czy efekt "DoubleMovement" jest aktywny
        if (_effects.Contains("DoubleMovement"))
        {
            Console.WriteLine("Efekt 'DoubleMovement' aktywny. Ruch podwójny.");
            newPosition = Map.Next(Position, direction);
            newPosition = Map.Next(newPosition, direction);
            _effects.Remove("DoubleMovement"); // Usuń efekt po użyciu
        }
        else
        {
            Console.WriteLine("Efekt 'DoubleMovement' nieaktywny. Ruch pojedynczy.");
            newPosition = Map.Next(Position, direction);
        }
        Console.WriteLine($"Nowa pozycja wyliczona: {newPosition}");

        if (!CanMoveTo(Map, newPosition))
        {
            Console.WriteLine($"Nie można poruszyć się na pozycję {newPosition}. Ruch zablokowany.");
            return;
        }

        // Aktualizacja pozycji
        Console.WriteLine($"Poruszam się z {Position} na {newPosition}.");
        Map.Remove(this, Position);
        Position = newPosition;
        Map.Add(this, Position);
        Console.WriteLine($"Nowa pozycja gracza: {Position}");
    }

    public void InteractField(BigMap map, string accessCode, Follower follower)
    {
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (!map.TryGetField(point, out var objectsAtPoint))
                continue;

            var unlockedField = objectsAtPoint.OfType<UnlockedField>().FirstOrDefault();
            if (unlockedField != null)
            {
                if (unlockedField.BlockedStatus && _keys.Contains(unlockedField.KeyId))
                {
                    if (accessCode == unlockedField.AccessCode)
                    {
                        unlockedField.SetBlockedStatus(false);
                        Console.WriteLine($"Pole {point} zostało odblokowane!");
                        // Sprawdź, czy to pole aktywuje followera
                        if (map.IsTriggerPoint(point))
                        {
                            follower.ActivateFollower(point);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Nieprawidłowy kod dostępu dla pola {point}.");
                    }
                }
                else
                {
                    Console.WriteLine($"Nie możesz odblokować tego pola. Czy masz odpowiedni klucz?");
                }
                return;
            }
        }

        Console.WriteLine("Nie znaleziono pola do odblokowania w pobliżu.");
    }

    public void InteractWithField(GameSession session, Map currentMap, Dictionary<string, Map> maps)
    {
        var teleportField = currentMap.At(Position).OfType<TeleportField>().FirstOrDefault();
        if (teleportField != null)
        {
            Console.WriteLine($"Teleport znaleziony: {teleportField.TargetMapName} -> {teleportField.TargetPosition}");

            if (maps.TryGetValue(teleportField.TargetMapName, out var targetMap))
            {
                session.ChangeMap(targetMap, teleportField.TargetPosition); // Zmiana mapy
            }
            else
            {
                Console.WriteLine($"Mapa docelowa '{teleportField.TargetMapName}' nie została znaleziona.");
            }
        }
        else
        {
            Console.WriteLine("Brak teleportu na bieżącej pozycji.");
        }
    }
    public void InteractKey(Map map)
    {
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (!map.TryGetField(point, out var objectsAtPoint))
                continue;

            var key = objectsAtPoint.OfType<Key>().FirstOrDefault();
            if (key != null)
            {
                if (_keys.Add(key.KeyId))
                {
                    map.Remove(key, point);
                    objectsAtPoint.Remove(key);
                    Console.WriteLine($"Podniosłeś klucz {key.KeyId} i został on usunięty z mapy!");
                }
                else
                {
                    Console.WriteLine($"Masz już ten klucz, nie można go podnieść ponownie.");
                }
                return;
            }
        }

        Console.WriteLine("Nie znaleziono klucza w pobliżu.");
    }

    public void InteractPotion(Map map)
    {
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (!map.TryGetField(point, out var objectsAtPoint))
                continue;

            var potion = objectsAtPoint.OfType<Potions>().FirstOrDefault();
            if (potion != null)
            {
                Inventory.AddItem(potion, 1);
                map.Remove(potion, point);
                objectsAtPoint.Remove(potion);
                Console.WriteLine($"Podniosłeś eliksir o efekcie '{potion.Effect}'!");
                return;
            }
        }

        Console.WriteLine("Nie znaleziono eliksiru w pobliżu.");
    }

    public void UsePotion(string effect)
    {
        if (string.IsNullOrEmpty(effect))
        {
            Console.WriteLine("Nazwa efektu nie może być pusta.");
            return;
        }

        var potionRecord = Inventory.InventoryRecords
            .FirstOrDefault(record => record.InventoryItem is Potions potion && potion.Effect == effect);

        if (potionRecord == null)
        {
            Console.WriteLine($"Nie posiadasz eliksiru o efekcie: {effect}.");
            return;
        }

        Console.WriteLine($"Aktywacja efektu: {effect}");
        _effects.Add(effect);

        potionRecord.ReduceQuantity(1);
        if (potionRecord.Quantity <= 0)
        {
            Inventory.InventoryRecords.Remove(potionRecord);
        }
    }

    private IEnumerable<Point> GetAdjacentPoints()
    {
        return new[]
        {
            new Point(Position.X, Position.Y + 1),
            new Point(Position.X, Position.Y - 1),
            new Point(Position.X - 1, Position.Y),
            new Point(Position.X + 1, Position.Y),
        };
    }
}