using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class GameSimulation
    {
        private readonly BigMap _map;
        private readonly Player _player;
        public List<string> DebugMessages { get; private set; }

        public GameSimulation(int width, int height)
        {
            _map = new BigMap(width, height);
            DebugMessages = new List<string>();

            // Dodajemy zablokowane pola
            _map.AddBlockedField(new Point(1, 1));
            _map.AddBlockedField(new Point(1, 2));
            _map.AddBlockedField(new Point(1, 3));

            // Dodajemy eliksiry
            _map.AddPotion(new Point(5, 5), "DoubleMovement");

            // Dodajemy pola do odblokowania i klucze
            _map.AddUnlockedField(new Point(2, 3), 1, "1111");
            _map.AddKey(new Point(6, 6), 1);

            // Tworzymy gracza
            _player = new Player("Hero");
            _player.InitMapAndPosition(_map, new Point(0, 0));
        }

        public List<List<char>> GetMapState()
        {
            var mapState = new List<List<char>>();

            for (int y = _map.SizeY - 1; y >= 0; y--)
            {
                var row = new List<char>();
                for (int x = 0; x < _map.SizeX; x++)
                {
                    var point = new Point(x, y);
                    if (_map.IsBlocked(point))
                        row.Add('X'); // Zablokowane pole
                    else if (_map.IsPotion(point))
                        row.Add('E'); // Eliksir
                    else if (_map.IsUnlockable(point))
                        row.Add('Y'); // Pole do odblokowania
                    else if (_map.IsKey(point))
                        row.Add('*'); // Klucz
                    else if (_player.Position.Equals(point))
                        row.Add('P'); // Gracz
                    else
                        row.Add(' '); // Puste pole
                }
                mapState.Add(row);
            }

            return mapState;
        }

        public void MovePlayer(string direction)
        {
            Direction? parsedDirection = direction.ToUpper() switch
            {
                "W" => Direction.Up,
                "A" => Direction.Left,
                "S" => Direction.Down,
                "D" => Direction.Right,
                _ => null
            };

            if (parsedDirection.HasValue)
            {
                _player.Go(parsedDirection.Value);
                DebugMessages.Add($"Player moved {parsedDirection.Value}.");
            }
            else
            {
                DebugMessages.Add("Invalid direction input.");
            }
        }

        public void InteractKey()
        {
            DebugMessages.Add("Attempting to pick up a key...");
            _player.InteractKey(_map);
        }

        public void InteractField(string accessCode)
        {
            DebugMessages.Add("Attempting to unlock a field...");
            _player.InteractField(_map, accessCode);
        }

        public void InteractPotion()
        {
            DebugMessages.Add("Attempting to pick up a potion...");
            _player.InteractPotion(_map);
        }
    }
}