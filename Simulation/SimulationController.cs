using Simulation;
using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SimulationController
{
    private readonly Player _player;
    private readonly Map _map;
    private readonly List<string> _debugMessages;
    private readonly IEnumerable<Npc> _npcs;

    public SimulationController(Player player, Map map, IEnumerable<Npc> npcs)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _map = map ?? throw new ArgumentNullException(nameof(map));
        _npcs = npcs ?? throw new ArgumentNullException(nameof(npcs));
        _debugMessages = new List<string>();
    }

    public void Run()
    {
        while (true)
        {
            try
            {
                Console.Clear();

                // Wyświetl komunikaty debugujące poniżej mapy
                Console.WriteLine("\n=== Komunikaty debugowania ===");
                foreach (var message in _debugMessages)
                {
                    Console.WriteLine(message);
                }
                _debugMessages.Clear(); // Czyszczenie komunikatów na koniec iteracji

                // Wyświetlanie instrukcji sterowania na ekranie
                DisplayControls();

                // Sprawdzanie NPC w pobliżu gracza
                CheckNearbyNpcs();

                // Obsługa wejścia użytkownika
                var input = Console.ReadKey(true).Key;
                if (!ProcessInput(input))
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _debugMessages.Add($"Błąd: {ex.Message}");
            }
        }
    }

    private void DisplayControls()
    {
        Console.WriteLine("\n=== Sterowanie ===");
        Console.WriteLine("W: Góra");
        Console.WriteLine("A: Lewo");
        Console.WriteLine("S: Dół");
        Console.WriteLine("D: Prawo");
        Console.WriteLine("Q: Zbierz klucz");
        Console.WriteLine("E: Odblokuj pole");
        Console.WriteLine("U: Zbierz eliksir");
        Console.WriteLine("I: Użyj eliksiru");
        Console.WriteLine("Enter: Zakończ symulację");
    }

    private void CheckNearbyNpcs()
    {
        foreach (var npc in _npcs)
        {
            var dialogue = npc.CheckAndSpeak(_player.Position);
            if (!string.IsNullOrEmpty(dialogue))
            {
                _debugMessages.Add(dialogue);
            }
        }
    }

    private bool ProcessInput(ConsoleKey input)
    {
        switch (input)
        {
            case ConsoleKey.Enter:
                Console.WriteLine("Symulacja się zakończyła. Do zobaczenia!");
                return false;

            case ConsoleKey.Q:
                _debugMessages.Add("Próba zebrania klucza...");
                _player.InteractKey(_map);
                break;

            case ConsoleKey.E:
                _debugMessages.Add("Próba odblokowania pola...");
                Console.WriteLine("Wprowadź kod odblokowujący pole, a następnie naciśnij enter:");
                string accessCode = Console.ReadLine();
                _player.InteractField((BigMap)_map, accessCode, null); // Assuming BigMap and no follower for simplicity
                break;

            case ConsoleKey.U:
                _debugMessages.Add("Próba zebrania eliksiru...");
                _player.InteractPotion(_map);
                break;

            case ConsoleKey.I:
                DisplayPotions();
                break;

            default:
                ProcessMovement(input);
                break;
        }

        return true;
    }

    private void DisplayPotions()
    {
        Console.WriteLine("Dostępne eliksiry w ekwipunku:");
        foreach (var record in _player.Inventory.InventoryRecords.Where(r => r.InventoryItem is Potions))
        {
            var potion = (Potions)record.InventoryItem;
            Console.WriteLine($"- {potion.Effect} (ilość: {record.Quantity})");
        }

        Console.WriteLine("Wprowadź nazwę efektu eliksiru do użycia:");
        string effect = Console.ReadLine();
        _player.UsePotion(effect);
    }

    private void ProcessMovement(ConsoleKey input)
    {
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
            _debugMessages.Add($"Gracz porusza się w kierunku {direction.Value}.");
            _player.Go(direction.Value);
        }
        else
        {
            _debugMessages.Add("Nieprawidłowy klawisz. Użyj W/A/S/D.");
        }
    }
}
