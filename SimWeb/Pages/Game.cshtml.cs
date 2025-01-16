using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simulation.Maps;


using Simulation;
using SimWeb;

public class GameModel : PageModel
{
    private readonly GameSession _gameSession;

    public Point PlayerPosition => _gameSession.PlayerPosition;

    public GameModel(GameSession gameSession)
    {
        _gameSession = gameSession;
    }



    public List<List<string>> GetMapRepresentation()
    {
        var map = new List<List<string>>();

        for (int y = 9; y >= 0; y--) // 10x10 mapa
        {
            var row = new List<string>();
            for (int x = 0; x < 10; x++)
            {
                string cellContent = " "; // Puste pole jako domyœlne

                if (_gameSession.PlayerPosition.Equals(new Point(x, y)))
                {
                    cellContent = "P"; // Pozycja gracza
                }
                else if (_gameSession.IsBlocked(x, y))
                {
                    cellContent = "X"; // Pole zablokowane
                }
                else if (_gameSession.IsPotion(x, y))
                {
                    cellContent = "E"; // Eliksir
                }
                else if (_gameSession.IsUnlockable(x, y))
                {
                    cellContent = "Y"; // Pole wymagaj¹ce klucza
                }
                else if (_gameSession.IsKey(x, y))
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
        // Nie wymaga dodatkowej logiki
    }

    public IActionResult OnPostMove(string direction)
    {
        Direction? parsedDirection = direction switch
        {
            "Up" => Direction.Up,
            "Down" => Direction.Down,
            "Left" => Direction.Left,
            "Right" => Direction.Right,
            _ => null // W przypadku nieprawid?owego kierunku
        };

        if (parsedDirection.HasValue)
        {
            _gameSession.MovePlayer(parsedDirection.Value);
        }

        return RedirectToPage();
    }
}