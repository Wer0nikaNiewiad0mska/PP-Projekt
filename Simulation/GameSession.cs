using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class GameSession
{
    private readonly BigMap _map;
    private readonly Player _player;

    public Point PlayerPosition => _player.Position;

    public GameSession(BigMap map)
    {
        _map = map ?? throw new ArgumentNullException(nameof(map));
        _player = new Player("Hero");
        _player.InitMapAndPosition(_map, new Point(0, 0));
    }

    private void InitializePlayer()
    {
        // Umieszczamy gracza na domyślnej pozycji
        _player.InitMapAndPosition(_map, new Point(0, 0));
    }

    public void MovePlayer(Direction direction)
    {
        _player.Go(direction);
    }
    public string GetDebugInfo()
    {
        return _player.ToString();
    }

    public bool IsBlocked(int x, int y) => _map.IsBlocked(new Point(x, y));
    public bool IsPotion(int x, int y) => _map.IsPotion(new Point(x, y));
    public bool IsUnlockable(int x, int y) => _map.IsUnlockable(new Point(x, y));
    public bool IsKey(int x, int y) => _map.IsKey(new Point(x, y));
}