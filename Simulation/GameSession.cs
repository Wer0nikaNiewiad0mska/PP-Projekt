using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class GameSession
{
    private BigMap _map;
    private Player _player;

    public Point PlayerPosition => _player.Position;

    public void Initialize(BigMap map, Player player)
    {
        _map = map ?? throw new ArgumentNullException(nameof(map));
        _player = player ?? throw new ArgumentNullException(nameof(player));

        if (!_map.Exist(_player.Position))
            throw new InvalidOperationException("Gracz musi być zainicjalizowany na mapie.");
    }

    public void MovePlayer(Direction direction)
    {
        _player.Go(direction);
    }

    public string GetDebugInfo()
    {
        return _player.ToString();
    }

    public bool IsBlocked(Point position) => _map.IsBlocked(position);
    public bool IsPotion(Point position) => _map.IsPotion(position);
    public bool IsUnlockable(Point position) => _map.IsUnlockable(position);
    public bool IsKey(Point position) => _map.IsKey(position);
}