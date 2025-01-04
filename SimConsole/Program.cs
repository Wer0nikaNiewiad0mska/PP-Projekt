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
            // Tworzymy mapę
            var map = new BigMap(10, 10);

            // Dodajemy zablokowane pola
            map.AddBlockedField(new Point(1, 1));
            map.AddBlockedField(new Point(1, 2));
            map.AddBlockedField(new Point(1, 3));
            map.AddBlockedField(new Point(3, 1));
            map.AddBlockedField(new Point(3, 2));
            map.AddBlockedField(new Point(3, 3));
            map.AddBlockedField(new Point(2, 1));

            // Dodajemy odblokowywane pola i klucze
            map.AddUnlockedField(new Point(2, 3), 1); // Pole wymagające klucza o ID 1
            map.AddKey(new Point(6, 6), 1);           // Klucz o ID 1
            Console.WriteLine("Klucz został dodany na mapę."); //do sprawdzania co nie działa sobie dodałam jbc
            map.AddUnlockedField(new Point(4, 4), 2);
            map.AddKey(new Point(2, 2), 2);
            Console.WriteLine("Klucz został dodany na mapę."); //tutaj też do sprawdzania gdzie bląd

            // Tworzymy wizualizację mapy
            var visualizer = new MapVisualizer(map);

            // Tworzymy gracza i NPC + ich pozycje na mapie
            var player = new Player("Hero");
            var npc1 = new Npc("Npc1", "Użyj tego klucza aby otworzyć pole Y, które blokuje ostatni klucz.", 'n');
            var npc2 = new Npc("Npc2", "Uważaj, nieznajomy.");

            player.InitMapAndPosition(map, new Point(0, 0));
            npc1.InitMapAndPosition(map, new Point(7, 6));
            npc2.InitMapAndPosition(map, new Point(3, 6));

            // Główna pętla gry
            Console.WriteLine("Użyj W/A/S/D do pruszenia się. Naciśnij Enter aby zakończyć.");
            while (true)
            {
                try
                {
                    Console.Clear();
                    visualizer.Draw();

                    // Sprawdzanie NPC w pobliżu gracza
                    foreach (var npc in new[] { npc1, npc2 })
                    {
                        var dialogue = npc.CheckAndSpeak(player.Position);
                        if (!string.IsNullOrEmpty(dialogue))
                        {
                            Console.WriteLine(dialogue);
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
                        player.InteractKey(map); // Próba zebrania klucza
                    }
                    else if (input == ConsoleKey.E)
                    {
                        player.InteractField(map); // Próba odblokowania pola
                    }
                    else
                    {
                        // Interpretacja kierunku ruchu
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
                            player.Go(direction.Value);
                        }
                        else
                        {
                            Console.WriteLine("Proszę użyj W/A/S/D do poruszenia.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Pojawił się błąd podczas rozgrywki: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Pojawił się błąd: {ex.Message}");
        }
    }
}