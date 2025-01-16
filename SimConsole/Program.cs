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
            // Inicjalizujemy grę
            var (map, player, npc1, npc2, visualizer) = InitializeGame();

            // Główna pętla gry
            Console.WriteLine("Użyj W/A/S/D do poruszania się. Naciśnij Q aby zebrać klucz, E aby odblokować pole, U aby zebrać eliksir, I aby użyć eliksiru, lub Enter aby zakończyć.");
            var debugMessages = new List<string>();

            while (true)
            {
                try
                {
                    Console.Clear();
                    visualizer.Draw();

                    // Wyświetl komunikaty debugujące poniżej mapy
                    Console.WriteLine("\n=== Komunikaty debugowania ===");
                    foreach (var message in debugMessages)
                    {
                        Console.WriteLine(message);
                    }
                    debugMessages.Clear(); // Czyszczenie komunikatów na koniec iteracji

                    // Sprawdzanie NPC w pobliżu gracza
                    foreach (var npc in new[] { npc1, npc2 })
                    {
                        var dialogue = npc.CheckAndSpeak(player.Position);
                        if (!string.IsNullOrEmpty(dialogue))
                        {
                            debugMessages.Add(dialogue);
                        }
                    }

                    // Obsługa wejścia użytkownika
                    var input = Console.ReadKey(true).Key;

                    if (input == ConsoleKey.Enter)
                    {
                        Console.WriteLine("Symulacja się zakończyła. Do zobaczenia!");
                        break;
                    }

                    if (input == ConsoleKey.Q)
                    {
                        debugMessages.Add("Próba zebrania klucza...");
                        player.InteractKey(map);
                    }
                    else if (input == ConsoleKey.E)
                    {
                        debugMessages.Add("Próba odblokowania pola...");
                        Console.WriteLine("Wprowadź kod odblokowujący pole, a następnie naciśnij enter:");
                        string accessCode = Console.ReadLine();
                        player.InteractField(map, accessCode);
                    }
                    else if (input == ConsoleKey.U)
                    {
                        debugMessages.Add("Próba zebrania eliksiru...");
                        player.InteractPotion(map);
                    }
                    else if (input == ConsoleKey.I)
                    {
                        Console.WriteLine("Dostępne eliksiry w ekwipunku:");
                        foreach (var record in player.Inventory.InventoryRecords.Where(r => r.InventoryItem is Potions))
                        {
                            var potion = (Potions)record.InventoryItem;
                            Console.WriteLine($"- {potion.Effect} (ilość: {record.Quantity})");
                        }

                        Console.WriteLine("Wprowadź nazwę efektu eliksiru do użycia:");
                        string effect = Console.ReadLine();
                        player.UsePotion(effect); // Wywołanie metody `UsePotion`
                    }
                    else
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
                            debugMessages.Add($"Gracz porusza się w kierunku {direction.Value}.");
                            player.Go(direction.Value);
                        }
                        else
                        {
                            debugMessages.Add("Nieprawidłowy klawisz. Użyj W/A/S/D.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    debugMessages.Add($"Błąd: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Pojawił się błąd: {ex.Message}");
        }
    }

    private static (BigMap map, Player player, Npc npc1, Npc npc2, MapVisualizer visualizer) InitializeGame()
    {
        // Tworzymy mapę
        var map = new BigMap(10, 10);

        // Dodajemy zablokowane pola
        MapRules.AddBlockedField(map.Fields, new Point(1, 1));
        MapRules.AddBlockedField(map.Fields, new Point(1, 2));
        MapRules.AddBlockedField(map.Fields, new Point(1, 3));
        MapRules.AddBlockedField(map.Fields, new Point(3, 1));
        MapRules.AddBlockedField(map.Fields, new Point(3, 2));
        MapRules.AddBlockedField(map.Fields, new Point(3, 3));
        MapRules.AddBlockedField(map.Fields, new Point(2, 1));

        // Dodajemy eliksiry
        MapRules.AddPotion(map.Fields, new Point(5, 5), "DoubleMovement");
        MapRules.AddPotion(map.Fields, new Point(8, 8), "DoubleMovement");

        // Dodajemy odblokowywane pola i klucze
        MapRules.AddUnlockedField(map.Fields, new Point(2, 3), 1, "1111"); // Pole wymagające klucza o ID 1 i kodu "1111"
        MapRules.AddKey(map.Fields, new Point(6, 6), 1);
        Console.WriteLine("Klucz 1 został dodany na mapę na pozycję (6, 6).");

        MapRules.AddUnlockedField(map.Fields, new Point(4, 4), 2, "2222"); // Pole wymagające klucza o ID 2 i kodu "2222"
        MapRules.AddKey(map.Fields, new Point(2, 2), 2);
        Console.WriteLine("Klucz 2 został dodany na mapę na pozycję (2, 2).");

        // Tworzymy wizualizację mapy
        var visualizer = new MapVisualizer(map);

        // Tworzymy gracza i NPC + ich pozycje na mapie
        var player = new Player("Hero");
        var npc1 = new Npc("Npc1", "Użyj tego klucza aby otworzyć pole Y, które blokuje ostatni klucz.", 'n');
        var npc2 = new Npc("Npc2", "Uważaj, nieznajomy.");

        player.InitMapAndPosition(map, new Point(0, 0));
        npc1.InitMapAndPosition(map, new Point(7, 6));
        npc2.InitMapAndPosition(map, new Point(3, 6));

        return (map, player, npc1, npc2, visualizer);
    }
}