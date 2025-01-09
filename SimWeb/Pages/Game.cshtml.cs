using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simulation.Maps;


using Simulation;

public class GameModel : PageModel
{
    public GameSimulation Simulation { get; private set; }
    public List<List<char>> MapState { get; private set; }
    public List<string> DebugMessages { get; private set; }

    public GameModel()
    {
        Simulation = new GameSimulation(10, 10);
        MapState = Simulation.GetMapState();
        DebugMessages = Simulation.DebugMessages;
    }

    public void OnPost(string command)
    {
        switch (command.ToUpper())
        {
            case "W":
            case "A":
            case "S":
            case "D":
                Simulation.MovePlayer(command);
                break;
            case "Q":
                Simulation.InteractKey();
                break;
            case "E":
                Simulation.InteractField("1111"); // Przyk³adowy kod
                break;
            case "U":
                Simulation.InteractPotion();
                break;
            default:
                Simulation.DebugMessages.Add("Invalid command.");
                break;
        }

        MapState = Simulation.GetMapState();
        DebugMessages = Simulation.DebugMessages;
    }
}