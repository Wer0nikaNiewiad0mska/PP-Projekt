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
                string cellContent = " "; // Puste pole jako domy?lne

                if (_gameSession.PlayerPosition.Equals(new Point(x, y)))
                {
                    cellContent = "P"; // Pozycja gracza
                }
                else if (_gameSession.IsBlocked(new Point(x, y)))
                {
                    cellContent = "X"; // Pole zablokowane
                }
                else if (_gameSession.IsPotion(new Point(x, y)))
                {
                    cellContent = "E"; // Eliksir
                }
                else if (_gameSession.IsUnlockable(new Point(x, y)))
                {
                    cellContent = "Y"; // Pole wymagaj?ce klucza
                }
                else if (_gameSession.IsKey(new Point(x, y)))
                {
                    cellContent = "*"; // Klucz
                }

                row.Add(cellContent);
            }
            map.Add(row);
        }

        return map;
    }

    public void OnGet()
    {
        // Domyœlna inicjalizacja
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
            DebugMessages.Add($"Gracz porusza siê w kierunku {parsedDirection.Value}.");
            _gameSession.MovePlayer(parsedDirection.Value);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostCollectKey()
    {
        DebugMessages.Add("Próba zebrania klucza...");
        // Dodaj logikê zbierania klucza
        return RedirectToPage();
    }

    public IActionResult OnPostUsePotion()
    {
        DebugMessages.Add("Próba u¿ycia eliksiru...");
        // Dodaj logikê u¿ywania eliksiru
        return RedirectToPage();
    }

    public IActionResult OnPostUnlockField(string accessCode)
    {
        DebugMessages.Add("Próba odblokowania pola...");
        // Dodaj logikê odblokowania pola
        return RedirectToPage();
    }

    public IActionResult OnPostInteractWithNpc()
    {
        DialogueMessage = "NPC: Witaj, podró¿niku!";
        return RedirectToPage();
    }
}