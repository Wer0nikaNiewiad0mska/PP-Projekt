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

    public string CurrentMapName => _gameSession.CurrentMap.GetType().Name; // Pobiera nazwê klasy mapy
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
                var position = new Simulation.Point(x, y); // Uzycie Simulation.Point

                string cellContent = "empty";

                if (_gameSession.PlayerPosition.Equals(position))
                    cellContent = "player";
                else if (_gameSession.CurrentMap.At(position).OfType<Npc>().Any())
                {
                    var npc = _gameSession.CurrentMap.At(position).OfType<Npc>().FirstOrDefault();
                    if (npc != null)
                    {
                        cellContent = $"npc:{npc.Name.ToLower()}"; // Dodajemy imiê NPC jako identyfikator
                    }
                }
                
                else if (_gameSession.IsBlocked(position))
                    cellContent = "blocked";
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
        // Sprawdź obecność NPC w sąsiednich polach
        var adjacentPoints = GetAdjacentPoints();
        foreach (var point in adjacentPoints)
        {
            if (_gameSession.IsNpc(point))
            {
                var npc = _gameSession.CurrentMap.At(point).OfType<Npc>().FirstOrDefault();
                if (npc != null)
                {
                    DialogueMessage = $"{npc.Name}: {npc.Dialogue}"; // Dodaj imię przed dialogiem
                    DebugMessages.Add($"Dialog z NPC {npc.Name}: {npc.Dialogue}");
                }
            }
        }
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
            DebugMessages.Add($"Gracz porusza się w kierunku {parsedDirection.Value}.");
            _gameSession.MovePlayer(parsedDirection.Value);

            // Sprawdź obecność NPC w sąsiednich polach po ruchu
            var adjacentPoints = GetAdjacentPoints();
            foreach (var point in adjacentPoints)
            {
                if (_gameSession.IsNpc(point))
                {
                    var npc = _gameSession.CurrentMap.At(point).OfType<Npc>().FirstOrDefault();
                    if (npc != null)
                    {
                        DialogueMessage = npc.Dialogue; // Ustaw dialog NPC
                        DebugMessages.Add($"Dialog z NPC: {DialogueMessage}");
                    }
                }
            }
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
        DebugMessages.Add("Próba aktywacji punktu...");
        var playerPosition = _gameSession.PlayerPosition;

        if (_gameSession.IsTriggerPoint(playerPosition))
        {
            _gameSession.ActivateFollower(playerPosition);
            DebugMessages.Add($"Follower aktywowany w punkcie: {playerPosition}.");
        }
        else
        {
            DebugMessages.Add("Gracz nie znajduje siê na punkcie aktywacyjnym.");
        }

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