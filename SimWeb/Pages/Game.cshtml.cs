using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simulation.Maps;


using Simulation;
using SimWeb;


public class GameModel : PageModel
{
    private readonly GameSession _gameSession;
    public List<string> DebugMessages { get; private set; } = new List<string>();
    public string DialogueMessage { get; private set; }

    public Point PlayerPosition => _gameSession.PlayerPosition;

    public GameModel(GameSession gameSession)
    {
        _gameSession = gameSession;
    }

    public List<List<string>> GetMapRepresentation()
    {
        var map = new List<List<string>>();

        for (int y = _gameSession.CurrentMap.SizeY - 1; y >= 0; y--)
        {
            var row = new List<string>();
            for (int x = 0; x < _gameSession.CurrentMap.SizeX; x++)
            {
                var position = new Simulation.Point(x, y); // U�ycie Simulation.Point

                string cellContent = "empty";

                if (_gameSession.PlayerPosition.Equals(position))
                    cellContent = "player";
                else if (_gameSession.IsBlocked(position))
                    cellContent = "blocked";
                else if (_gameSession.CurrentMap.At(position).OfType<Npc>().Any())
                {
                    var npc = _gameSession.CurrentMap.At(position).OfType<Npc>().FirstOrDefault();
                    if (npc != null)
                    {
                        cellContent = $"npc:{npc.Name.ToLower()}"; // Dodajemy imi� NPC jako identyfikator
                    }
                }
                else if (_gameSession.IsPotion(position))
                    cellContent = "potion";
                else if (_gameSession.IsUnlockable(position))
                    cellContent = "unlockable";
                else if (_gameSession.IsKey(position))
                    cellContent = "key";
                else if (_gameSession.IsTeleport(position))
                    cellContent = "teleport";
                else if (_gameSession.IsFollower(position))
                    cellContent = "follower";
                else if (_gameSession.IsTriggerPoint(position))
                    cellContent = "trigger";

                row.Add(cellContent);
            }
            map.Add(row);
        }

        return map;
    }
    public void OnGet()
    {
        // Domy?lna inicjalizacja widoku
    }

    public IActionResult OnPostMove(string direction)
    {
        var parsedDirection = direction switch
        {
            "Up" => Direction.Up,
            "Down" => Direction.Down,
            "Left" => Direction.Left,
            "Right" => Direction.Right,
            _ => (Direction?)null
        };

        if (parsedDirection.HasValue)
        {
            DebugMessages.Add($"Gracz porusza si? w kierunku {parsedDirection.Value}.");
            _gameSession.MovePlayer(parsedDirection.Value);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostCollectKey()
    {
        DebugMessages.Add("Pr�ba zebrania klucza...");
        // Logika zbierania klucza, na podstawie pobliskich p�l
        var adjacentPoints = GetAdjacentPoints();
        foreach (var point in adjacentPoints)
        {
            if (_gameSession.IsKey(point))
            {
                DebugMessages.Add($"Zebrano klucz z pozycji {point}.");
                return RedirectToPage();
            }
        }
        DebugMessages.Add("Nie znaleziono klucza w pobli?u.");
        return RedirectToPage();
    }

    public IActionResult OnPostUsePotion(string potionEffect)
    {
        if (!string.IsNullOrEmpty(potionEffect))
        {
            DebugMessages.Add($"Pr�ba u?ycia eliksiru: {potionEffect}...");
            // Logika u?ycia eliksiru
        }
        else
        {
            DebugMessages.Add("Nie podano efektu eliksiru.");
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUnlockField(string accessCode)
    {
        if (!string.IsNullOrEmpty(accessCode))
        {
            DebugMessages.Add("Pr�ba odblokowania pola...");
            // Logika odblokowania pola, na podstawie pobliskich punkt�w
            var adjacentPoints = GetAdjacentPoints();
            foreach (var point in adjacentPoints)
            {
                if (_gameSession.IsUnlockable(point))
                {
                    DebugMessages.Add($"Pole odblokowane na pozycji {point} przy u?yciu kodu: {accessCode}.");
                    return RedirectToPage();
                }
            }
            DebugMessages.Add("Nie znaleziono pola do odblokowania w pobli?u.");
        }
        else
        {
            DebugMessages.Add("Nie podano kodu dost?pu.");
        }
        return RedirectToPage();
    }

    public IActionResult OnPostInteractWithNpc()
    {
        DialogueMessage = _gameSession.GetNpcDialogue(); // Pobierz dialog NPC
        DebugMessages.Add($"Dialog z NPC: {DialogueMessage}");
        return RedirectToPage();
    }

    public IActionResult OnPostActivateFollower()
    {
        DebugMessages.Add("Pr�ba aktywacji punktu...");
        var playerPosition = _gameSession.PlayerPosition;

        if (_gameSession.IsTriggerPoint(playerPosition))
        {
            _gameSession.ActivateFollower(playerPosition);
            DebugMessages.Add($"Follower aktywowany w punkcie: {playerPosition}.");
        }
        else
        {
            DebugMessages.Add("Gracz nie znajduje si� na punkcie aktywacyjnym.");
        }

        return RedirectToPage();
    }

    public IActionResult OnPostCollectPotion()
    {
        DebugMessages.Add("Pr�ba zebrania eliksiru...");

        // Pobierz punkty w pobli�u gracza
        var adjacentPoints = GetAdjacentPoints();

        foreach (var point in adjacentPoints)
        {
            if (_gameSession.IsPotion(point))
            {
                DebugMessages.Add($"Zebrano eliksir z pozycji {point}.");
                _gameSession.Player.InteractPotion(_gameSession.CurrentMap);
                return RedirectToPage();
            }
        }

        DebugMessages.Add("Nie znaleziono eliksiru w pobli�u.");
        return RedirectToPage();
    }

    public IActionResult OnPostInteractWithTeleport()
    {
        DebugMessages.Add("Sprawdzanie teleportu na obecnej pozycji...");

        _gameSession.Player.InteractWithField(
            _gameSession,
            _gameSession.CurrentMap,
            _gameSession.Maps
        );

        return RedirectToPage();
    }

    private IEnumerable<Point> GetAdjacentPoints()
    {
        var current = _gameSession.PlayerPosition;
        return new[]
        {
            new Point(current.X, current.Y + 1),
            new Point(current.X, current.Y - 1),
            new Point(current.X - 1, current.Y),
            new Point(current.X + 1, current.Y),
        };
    }
}