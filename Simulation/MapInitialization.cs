﻿using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public static class GameInitialization
{
    public static (Map bigMap, Map secondMap, Player player, Dictionary<string, Map>, Follower follower) InitializeGame()
    {
        var bigMap = new BigMap(10, 10);
        var secondMap = new SecondMap(10, 10);

        var player = new Player("Hero");

        // Ustawienia mapy BigMap
        bigMap.AddBlockedField(new Point(0, 1));
        bigMap.AddBlockedField(new Point(1,1));
        bigMap.AddBlockedField(new Point(2,1));
        bigMap.AddBlockedField(new Point(6, 8));
        bigMap.AddBlockedField(new Point(6, 7));
        bigMap.AddBlockedField(new Point(6, 6));
        bigMap.AddBlockedField(new Point(6, 5));
        bigMap.AddBlockedField(new Point(6, 4));
        bigMap.AddBlockedField(new Point(6, 3));
        bigMap.AddBlockedField(new Point(6, 2));
        bigMap.AddBlockedField(new Point(6, 1));
        bigMap.AddBlockedField(new Point(6, 0));

        bigMap.AddBlockedField(new Point(8, 8));
        bigMap.AddBlockedField(new Point(9, 8));

        bigMap.AddBlockedField(new Point(8, 6));

        bigMap.AddBlockedField(new Point(7, 5));
        bigMap.AddBlockedField(new Point(8, 5));
        
        bigMap.AddBlockedField(new Point(8, 3));
        bigMap.AddBlockedField(new Point(9, 2));
        bigMap.AddBlockedField(new Point(8, 2));
        


        // Dodaj followera
        var triggerPoint = new Point(8, 9); // Punkt, którego odblokowanie aktywuje followera
        var follower = new Follower("Wercia", triggerPoint);
        bigMap.Add(follower, new Point(9, 9)); // Początkowa pozycja followera
        follower.InitMapAndPosition(bigMap, new Point(9, 9));

        // Dodaj triggerujące pole na mapie
        bigMap.AddTriggerPoint(triggerPoint);

        // Dodanie NPC na mapie
        var Olamo = new Npc("Olamo", "Hej! Widziałam jak Weronika wchodzi do tego labiryntu..", 'O');
        Olamo.InitMapAndPosition(bigMap, new Point(4, 4));

        var npc11 = new Npc("Student1", "Kiedy zaczniesz uczyć się do sesji?", 'S');
        npc11.InitMapAndPosition(bigMap, new Point(1, 2));

        var npc12 = new Npc("Student2", "Jak tylko przestanę płakać", 's');
        npc12.InitMapAndPosition(bigMap, new Point(0, 2));

        // DRUGA MAPA
        secondMap.AddBlockedField(new Point(0, 3)); 
        secondMap.AddBlockedField(new Point(1, 3));
        secondMap.AddBlockedField(new Point(2, 3));
        secondMap.AddBlockedField(new Point(2, 2));
        secondMap.AddBlockedField(new Point(3, 2));
        secondMap.AddBlockedField(new Point(4, 2));
        secondMap.AddBlockedField(new Point(5, 2));
        secondMap.AddBlockedField(new Point(6, 2));
        secondMap.AddBlockedField(new Point(7, 2));
        secondMap.AddBlockedField(new Point(9, 2));
        secondMap.AddBlockedField(new Point(7, 3));
        secondMap.AddBlockedField(new Point(7, 5));
        secondMap.AddBlockedField(new Point(8, 5));
        secondMap.AddBlockedField(new Point(9, 5));
        secondMap.AddBlockedField(new Point(6, 5));
        secondMap.AddBlockedField(new Point(5, 5));
        secondMap.AddBlockedField(new Point(4, 5));
        secondMap.AddBlockedField(new Point(5, 4));
        secondMap.AddBlockedField(new Point(4, 4));
        secondMap.AddBlockedField(new Point(3, 5));
        secondMap.AddBlockedField(new Point(2, 5));
        secondMap.AddBlockedField(new Point(1, 5));
        secondMap.AddBlockedField(new Point(0, 7));
        secondMap.AddBlockedField(new Point(1, 7));
        secondMap.AddBlockedField(new Point(2, 7));
        secondMap.AddBlockedField(new Point(3, 7));
        secondMap.AddBlockedField(new Point(4, 7));
        secondMap.AddBlockedField(new Point(5, 7));
        secondMap.AddBlockedField(new Point(6, 7));
        secondMap.AddBlockedField(new Point(7, 7));
        secondMap.AddBlockedField(new Point(9, 7));
        secondMap.AddBlockedField(new Point(9, 8));
        secondMap.AddBlockedField(new Point(9, 9));
        secondMap.AddBlockedField(new Point(9, 0));
        secondMap.AddBlockedField(new Point(9, 1));






        // Dodaj pole teleportacyjne na BigMap
        var teleportToSecond = new TeleportField(new Point(9, 0), "SecondMap", new Point(0, 0));
        teleportToSecond.InitMapAndPosition(bigMap, new Point(9, 0));

        var npc2 = new Npc("RandomGuy", "Wasi znajomi czekają na was po drugiej stronie tego labiryntu", 'R');
        npc2.InitMapAndPosition(secondMap, new Point(8, 0));

        var filip = new Npc("Filip", "Co wam tak długo zeszło?", 'F');
        filip.InitMapAndPosition(secondMap, new Point(0, 9));

        var wiktor = new Npc("Wiktor", "No w końcu jesteście!", 'W');
        wiktor.InitMapAndPosition(secondMap, new Point(1, 9));

        var npc1 = new Npc("RandomGirl", "Nie wiem jak zawrócić..", 'r');
        npc1.InitMapAndPosition(secondMap, new Point(9, 6));

        // Dodaj pole teleportacyjne na SecondMap
        var teleportToBig = new TeleportField(new Point(0, 1), "BigMap", new Point(9, 1));
        teleportToBig.InitMapAndPosition(secondMap, new Point(0, 1));

        // Dodaj gracza na BigMap
        player.InitMapAndPosition(bigMap, new Point(0, 0));

        // Rejestracja map
        var maps = new Dictionary<string, Map>
        {
            { "BigMap", bigMap },
            { "SecondMap", secondMap }
        };

        return (bigMap, secondMap, player, maps, follower);
    }
}
