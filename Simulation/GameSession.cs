using Simulation;
using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;
public class GameSession
{
    private Map _currentMap;
    private Player _player;
    private Dictionary<string, Map> _maps;
    private Follower _follower;
    public Player Player => _player;
    public Point PlayerPosition => _player.Position;
    public Map CurrentMap => _currentMap; // Dostęp do obecnej mapy
    public Dictionary<string, Map> Maps => _maps; // Dostęp do wszystkich map


    public void Initialize(Map initialMap, Player player, Dictionary<string, Map> maps, Follower follower)
    {
        _currentMap = initialMap ?? throw new ArgumentNullException(nameof(initialMap));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _maps = maps ?? throw new ArgumentNullException(nameof(maps));
        _follower = follower ?? throw new ArgumentNullException(nameof(follower));

        if (!_currentMap.Exist(_player.Position))
            throw new InvalidOperationException("Gracz musi być zainicjalizowany na mapie.");
    }

    public string GetNpcDialogue()
    {
        foreach (var point in GetAdjacentPoints())
        {
            if (_currentMap.IsNpc(point)) // Sprawdzamy, czy na sąsiednim polu jest NPC
            {
                if (_currentMap.TryGetField(point, out var mappableObjects))
                {
                    var npc = mappableObjects.OfType<Npc>().FirstOrDefault();
                    if (npc != null)
                    {
                        return npc.CheckAndSpeak(PlayerPosition); // Zwracamy dialog NPC
                    }
                }
            }
        }

        return "Brak NPC w pobliżu."; // Zwraca domyślny komunikat, jeśli nie znaleziono NPC
    }
    public void MovePlayer(Direction direction)
    {
        var previousPlayerPosition = _player.Position;

        // Przesuń gracza
        _player.Go(direction);

        // Automatyczna interakcja z teleportem
        _player.InteractWithField(this, _currentMap, _maps);

        // Porusz followerem, jeśli aktywowany
        _follower?.FollowPlayer(previousPlayerPosition, _player, _currentMap);
    }

    public void ActivateFollower(Point position)
    {
        if (_follower.TriggerPoint == position)
        {
            _follower.ActivateFollower(position);
            Console.WriteLine("Follower został aktywowany.");
        }
        else
        {
            Console.WriteLine("Follower nie został aktywowany - gracz nie jest na trigger point.");
        }
    }
    public void ChangeMap(Map targetMap, Point targetPosition)
    {
        if (_currentMap == targetMap)
        {
            Console.WriteLine("Gracz już znajduje się na tej mapie.");
            return;
        }

        // Usuń gracza z obecnej mapy
        _currentMap.Remove(_player, _player.Position);

        // Przełącz na nową mapę
        _currentMap = targetMap;

        // Ustaw gracza na nowej mapie w odpowiedniej pozycji
        _player.InitMapAndPosition(_currentMap, targetPosition);

        // Wyczyść ekran i odśwież widok
        Console.Clear();
        Console.WriteLine($"Gracz został teleportowany na mapę: {_currentMap.GetType().Name}");
        UpdateMapView();
    }
    public void UpdateMapView()
    {
        Console.Clear();
        Console.WriteLine($"Aktualna mapa: {_currentMap.GetType().Name}");

        for (int y = _currentMap.SizeY - 1; y >= 0; y--)
        {
            for (int x = 0; x < _currentMap.SizeX; x++)
            {
                var objectsAtPosition = _currentMap.At(new Point(x, y));
                char symbol = '.';

                if (objectsAtPosition.Contains(_player))
                    symbol = 'P';
                else if (_currentMap.IsBlocked(new Point(x, y)))
                    symbol = 'X';
                else if (_currentMap.IsPotion(new Point(x, y)))
                    symbol = 'O';
                else if (_currentMap.IsKey(new Point(x, y)))
                    symbol = 'K';
                else if (_currentMap.IsTeleport(new Point(x, y)))
                    symbol = 'T';

                Console.Write(symbol + " ");
            }
            Console.WriteLine();
        }
    }
    public void ResetMapState()
    {
        Console.Clear(); // Wyczyszczenie widoku konsoli
        Console.WriteLine("Stan mapy został zresetowany.");
    }

    public string GetDebugInfo()
    {
        return _player.ToString();
    }

    public bool IsFollower(Point position) => _follower?.Position == position;
    public bool IsBlocked(Point position) => _currentMap.IsBlocked(position);
    public bool IsPotion(Point position) => _currentMap.IsPotion(position);
    public bool IsUnlockable(Point position) => _currentMap.IsUnlockable(position);
    public bool IsKey(Point position) => _currentMap.IsKey(position);
    public bool IsTeleport(Point position) => _currentMap.At(position).OfType<TeleportField>().Any();

    public bool IsTriggerPoint(Point position) => _currentMap.IsTriggerPoint(position);

    public bool IsNpc(Point position)
    {
        return CurrentMap.At(position).OfType<Npc>().Any();
    }


    private IEnumerable<Point> GetAdjacentPoints()
    {
        var current = _player.Position;
        return new[]
        {
        new Point(current.X, current.Y + 1),
        new Point(current.X, current.Y - 1),
        new Point(current.X - 1, current.Y),
        new Point(current.X + 1, current.Y),
    };
    }
}