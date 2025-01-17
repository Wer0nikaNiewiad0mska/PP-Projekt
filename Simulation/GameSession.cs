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

    public Point PlayerPosition => _player.Position;

    public void Initialize(Map initialMap, Player player, Dictionary<string, Map> maps, Follower follower)
    {
        _currentMap = initialMap ?? throw new ArgumentNullException(nameof(initialMap));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _maps = maps ?? throw new ArgumentNullException(nameof(maps));
        _follower = follower ?? throw new ArgumentNullException(nameof(follower));

        if (!_currentMap.Exist(_player.Position))
            throw new InvalidOperationException("Gracz musi być zainicjalizowany na mapie.");
    }

    public void MovePlayer(Direction direction)
    {
        var previousPlayerPosition = _player.Position; // Zachowaj poprzednią pozycję gracza

        // Przesuń gracza
        _player.Go(direction);

        // Sprawdź interakcje
        _player.InteractWithField(this, _currentMap, _maps);

        // Porusz followerem, jeśli istnieje
        _follower?.FollowPlayer(previousPlayerPosition, _player, _currentMap);

        UpdateMapView();
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

    public bool IsBlocked(Point position) => _currentMap.IsBlocked(position);
    public bool IsPotion(Point position) => _currentMap.IsPotion(position);
    public bool IsUnlockable(Point position) => _currentMap.IsUnlockable(position);
    public bool IsKey(Point position) => _currentMap.IsKey(position);
    public bool IsTeleport(Point position) => _currentMap.At(position).OfType<TeleportField>().Any();
}