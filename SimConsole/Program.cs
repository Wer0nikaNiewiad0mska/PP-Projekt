using System.Drawing;
using System.Numerics;
using Simulation;
using Simulation.Maps;
using Point = Simulation.Point;

namespace SimConsole;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            var (bigMap, secondMap, player, maps) = GameInitialization.InitializeGame();
            var session = new GameSession();
            session.Initialize(bigMap, player, maps);

            // Główna pętla gry
            RunGameLoop(session);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd: {ex.Message}");
        }
    }

    private static void RunGameLoop(GameSession session)
    {
        Console.WriteLine("Użyj W/A/S/D do poruszania się. Naciśnij Enter aby zakończyć.");
        while (true)
        {
            // Odśwież mapę i wyświetl widok
            session.UpdateMapView();

            Console.WriteLine("Pozycja gracza: " + session.PlayerPosition);

            var input = Console.ReadKey(true).Key;
            if (input == ConsoleKey.Enter) break;

            Direction? direction = input switch
            {
                ConsoleKey.W => Direction.Up,
                ConsoleKey.A => Direction.Left,
                ConsoleKey.S => Direction.Down,
                ConsoleKey.D => Direction.Right,
                _ => null
            };

            if (direction.HasValue)
            {
                session.MovePlayer(direction.Value);
            }
        }
    }
}