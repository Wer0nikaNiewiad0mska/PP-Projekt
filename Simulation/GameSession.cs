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

    public GameSession()
    {
        _map = new BigMap(10, 10);
        _player = new Player("Hero");
        Initialize();
    }

    private void Initialize()
    {
        // Tworzymy przykładową mapę z obiektami
        _map.AddBlockedField(new Point(5, 5));
        _map.AddUnlockedField(new Point(6, 5), 1, "code123");
        _map.AddKey(new Point(7, 5), 1);
        _map.AddPotion(new Point(8, 5), "SpeedBoost");

        // Tworzymy NPC
        var npc = new Npc("Gandalf", "Witaj, podróżniku! Czego szukasz?");
        npc.InitMapAndPosition(_map, new Point(4, 5));

        // Umieszczamy gracza na mapie
        _player.InitMapAndPosition(_map, new Point(0, 0));
    }

    public void MovePlayer(Direction direction)
    {
        _player.Go(direction);
    }

    public string GetDebugInfo()
    {
        return $"Gracz: {_player.Position}";
    }

    public bool IsBlocked(int x, int y) => _map.IsBlocked(new Point(x, y));
    public bool IsPotion(int x, int y) => _map.IsPotion(new Point(x, y));
    public bool IsUnlockable(int x, int y) => _map.IsUnlockable(new Point(x, y));
    public bool IsKey(int x, int y) => _map.IsKey(new Point(x, y));
}