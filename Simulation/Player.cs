using Simulation.Maps;

namespace Simulation;

public class Player : Creature
{
    public InventorySystem Inventory { get; } = new(); // Ekwipunek gracza
    public override char Symbol => 'P'; // Stały symbol gracza
    public Player(string name) : base(name) { }

    public void Go(Direction direction)
    {
        if (Map is not BigMap && Map is not SecondMap)
            throw new InvalidOperationException("Map is not set or is not a valid type (BigMap or SecondMap).");

        Console.WriteLine($"Gracz: {Name}, obecna pozycja: {Position}, próba ruchu w kierunku: {direction}");

        Point newPosition;

        newPosition = Map.Next(Position, direction);

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