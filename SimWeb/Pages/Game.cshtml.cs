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
                else if (_gameSession.IsBlocked(x, y))
                    cellContent = "blocked";
                else if (_gameSession.IsPotion(x, y))
                    cellContent = "potion";
                else if (_gameSession.IsUnlockable(x, y))
                    cellContent = "unlockable";
                else if (_gameSession.IsKey(x, y))
                    cellContent = "key";

                row.Add(cellContent);
            }
            map.Add(row);
        }

        return map;
    }

    public void OnGet()
    {
        // Domy�lna inicjalizacja
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
            DebugMessages.Add($"Gracz porusza si� w kierunku {parsedDirection.Value}.");
            _gameSession.MovePlayer(parsedDirection.Value);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostCollectKey()
    {
        DebugMessages.Add("Pr�ba zebrania klucza...");
        // Dodaj logik� zbierania klucza
        return RedirectToPage();
    }

    public IActionResult OnPostUsePotion()
    {
        DebugMessages.Add("Pr�ba u�ycia eliksiru...");
        // Dodaj logik� u�ywania eliksiru
        return RedirectToPage();
    }

    public IActionResult OnPostUnlockField(string accessCode)
    {
        DebugMessages.Add("Pr�ba odblokowania pola...");
        // Dodaj logik� odblokowania pola
        return RedirectToPage();
    }

    public IActionResult OnPostInteractWithNpc()
    {
        DialogueMessage = "NPC: Witaj, podr�niku!";
        return RedirectToPage();
    }
}