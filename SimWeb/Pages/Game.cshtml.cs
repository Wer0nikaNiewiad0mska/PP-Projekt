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