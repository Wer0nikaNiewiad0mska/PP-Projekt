using Simulation.Maps;
using System.Drawing;
using System.Numerics;

namespace Simulation;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Sprawdzenie, czy dialogi i imiona są poprawnie tworzone === \n");
        Npc npc1 = new Npc("Ciri", "Sprawdzenie, czy umie mówić", 'C'); // dialog NPC
        Console.WriteLine(npc1.Speak());
        Console.WriteLine($"Symbol NPC: {npc1.Symbol}");

        Npc npc2 = new Npc("         Aaaaaaaaaa                               ", "Sprawdzam, czy zrobi dobrze imie postaci");
        Console.WriteLine(npc2.Speak());



        Console.WriteLine("\n=== Poruszanie się gracza na mapie i pojawianie postaci ===\n");
        BigMap map = new BigMap(10, 10);
        Player player1 = new Player("Chomik");
        player1.InitMapAndPosition(map, new Point(5, 5));

        player1.Go(Direction.Up);
        Console.WriteLine($"Pozycja gracza {player1.Name}: {player1.Position.X}, {player1.Position.Y}");
        Console.WriteLine($"Pozycja npc {npc1.Name}: {npc1.Position.X}, {npc1.Position.Y}");



        Console.WriteLine("\n=== TEST BLOKADY RUCHU NA POLE CREATURE ===");
        Player player2 = new Player("Kot");
        Npc npc3 = new Npc("Pies", "Hau Hau");
        player2.InitMapAndPosition(map, new Point(3, 3));  // Gracz startuje na (3, 3)
        npc3.InitMapAndPosition(map, new Point(4, 3));     // NPC na (4, 3)

        // Próba wejścia na pole NPC
        player2.Go(Direction.Right);  // Powinno być zablokowane
        player2.Go(Direction.Up);     // Powinno się udać

        // Gracz powinien pozostać na (3, 4) po zablokowanym ruchu
        Console.WriteLine($"Pozycja gracza po blokadzie: {player2.Position.X}, {player2.Position.Y}");
    }
}
