using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class GameSession
{
    private readonly BigMap _map;
    private readonly Player _player;

    public GameSession(int mapWidth, int mapHeight, string playerName)
    {
        _map = new BigMap(mapWidth, mapHeight);
        _player = new Player(playerName);
    }

    public void Initialize()
    {
        // Tworzymy przykładową mapę z obiektami
        Console.WriteLine("Inicjalizacja gry...");

        // Dodajemy blokowane i odblokowane pola
        _map.AddBlockedField(new Point(5, 5));
        _map.AddUnlockedField(new Point(6, 5), 1, "code123");

        // Dodajemy klucz
        _map.AddKey(new Point(7, 5), 1);

        // Dodajemy eliksir
        _map.AddPotion(new Point(8, 5), "SpeedBoost");

        // Tworzymy NPC
        var npc = new Npc("Gandalf", "Witaj, podróżniku! Czego szukasz?");
        npc.InitMapAndPosition(_map, new Point(4, 5));

        // Umieszczamy gracza na mapie
        _player.InitMapAndPosition(_map, new Point(0, 0));

        Console.WriteLine("Gra została zainicjowana!");
    }

    public void Start()
    {
        // Prosta pętla gry
        while (true)
        {
            Console.WriteLine("\n--- Tura ---");
            Console.WriteLine($"Pozycja gracza: {_player.Position}");
            Console.WriteLine("Co chcesz zrobić?");
            Console.WriteLine("1. Ruch (W, S, A, D)");
            Console.WriteLine("2. Interakcja z polem");
            Console.WriteLine("3. Interakcja z kluczem");
            Console.WriteLine("4. Interakcja z eliksirem");
            Console.WriteLine("5. Sprawdzenie NPC");
            Console.WriteLine("6. Zakończ grę");

            var input = Console.ReadLine();
            if (input == "1")
            {
                // Ruch gracza
                Console.WriteLine("Wybierz kierunek (W = góra, S = dół, A = lewo, D = prawo):");
                var direction = Console.ReadLine()?.ToUpper();
                if (direction == "W") _player.Go(Direction.Up);
                else if (direction == "S") _player.Go(Direction.Down);
                else if (direction == "A") _player.Go(Direction.Left);
                else if (direction == "D") _player.Go(Direction.Right);
                else
                {
                    Console.WriteLine("Nieznany kierunek.");
                }
            }
            else if (input == "2")
            {
                // Interakcja z polem
                Console.WriteLine("Wprowadź kod dostępu do odblokowanego pola:");
                var accessCode = Console.ReadLine();
                _player.InteractField(_map, accessCode);
            }
            else if (input == "3")
            {
                // Interakcja z kluczem
                _player.InteractKey(_map);
            }
            else if (input == "4")
            {
                // Interakcja z eliksirem
                _player.InteractPotion(_map);
            }
            else if (input == "5")
            {
                // Sprawdzanie dialogu z NPC
                var npc = _map.At(new Point(4, 5)).OfType<Npc>().FirstOrDefault();
                if (npc != null)
                {
                    var dialogue = npc.CheckAndSpeak(_player.Position);
                    if (!string.IsNullOrEmpty(dialogue))
                    {
                        Console.WriteLine(dialogue);
                    }
                    else
                    {
                        Console.WriteLine("Nie jesteś wystarczająco blisko NPC.");
                    }
                }
                else
                {
                    Console.WriteLine("Brak NPC w pobliżu.");
                }
            }
            else if (input == "6")
            {
                Console.WriteLine("Gra zakończona.");
                break;
            }
            else
            {
                Console.WriteLine("Nieznana opcja.");
            }
        }
    }
}