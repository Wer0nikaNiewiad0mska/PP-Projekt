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

        for (int y = 9; y >= 0; y--)
        {
            var row = new List<string>();
            for (int x = 0; x < 10; x++)
            {
                string cellContent = "empty";

                if (_gameSession.PlayerPosition.Equals(new Point(x, y)))
                    cellContent = "player";
                else if (_gameSession.IsBlocked(new Point(x, y)))
                    cellContent = "blocked";
                else if (_gameSession.IsPotion(new Point(x, y)))
                    cellContent = "potion";
                else if (_gameSession.IsUnlockable(new Point(x, y)))
                    cellContent = "unlockable";
                else if (_gameSession.IsKey(new Point(x, y)))
                    cellContent = "key";
                else if (_gameSession.IsTeleport(new Point(x, y))) 
                    cellContent = "teleport";
                else if (_gameSession.IsFollower(new Point(x, y))) // Nowy warunek
                    cellContent = "follower";

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
        DebugMessages.Add("Próba zebrania klucza...");
        // Logika zbierania klucza, na podstawie pobliskich pól
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
            DebugMessages.Add($"Próba u?ycia eliksiru: {potionEffect}...");
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
            DebugMessages.Add("Próba odblokowania pola...");
            // Logika odblokowania pola, na podstawie pobliskich punktów
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
        DialogueMessage = "Brak NPC w pobli?u."; // Domy?lna wiadomo??
        // Logika interakcji z NPC
        var adjacentPoints = GetAdjacentPoints();
        foreach (var point in adjacentPoints)
        {
            if (_gameSession.IsBlocked(point)) // Zak?adamy, ?e NPC s? na polach zablokowanych
            {
                DialogueMessage = "NPC: Witaj, podró?niku!";
                break;
            }
        }
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