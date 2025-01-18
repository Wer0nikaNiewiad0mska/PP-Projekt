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
        bigMap.AddBlockedField(new Point(7, 8));
        bigMap.AddBlockedField(new Point(7, 7));
        bigMap.AddBlockedField(new Point(8, 7));
        //bigMap.AddBlockedField(new Point(9, 7));


        bigMap.AddBlockedField(new Point(9, 2));
        bigMap.AddBlockedField(new Point(8, 2));
        bigMap.AddBlockedField(new Point(7, 2));
        //bigMap.AddBlockedField(new Point(7, 1));

        bigMap.AddUnlockedField(new Point(7, 9), 1, "1");
        bigMap.AddKey(new Point(4, 2), 1);

        bigMap.AddUnlockedField(new Point(7, 0), 4, "");
        bigMap.AddKey(new Point(8, 8), 4);


        bigMap.AddPotion(new Point(5, 5), "DoubleMovement");
        // Dodaj followera
        var triggerPoint = new Point(8, 9); // Punkt, którego odblokowanie aktywuje followera
        var follower = new Follower("Wercia", triggerPoint);
        bigMap.Add(follower, new Point(9, 9)); // Początkowa pozycja followera
        follower.InitMapAndPosition(bigMap, new Point(9, 9));

        // Dodaj triggerujące pole na mapie
        bigMap.AddTriggerPoint(triggerPoint);

        //Dodanie NPC na mapie
        var Olamo = new Npc("OlaMo", "Hej! Chyba widziałam jak Weronika wchodzi do tego miejsca i wpisuje 1 na drzwiach..", 'O');
        Olamo.InitMapAndPosition(bigMap, new Point(4, 4));

        //DRUGA MAPA
        secondMap.AddBlockedField(new Point(0, 2));
        secondMap.AddBlockedField(new Point(1, 2));
        secondMap.AddBlockedField(new Point(2, 2));
        secondMap.AddBlockedField(new Point(3, 2));
        secondMap.AddBlockedField(new Point(4, 2));
        secondMap.AddBlockedField(new Point(5, 2));
        secondMap.AddBlockedField(new Point(6, 2));
        secondMap.AddBlockedField(new Point(7, 2));
        secondMap.AddBlockedField(new Point(7, 3));
        secondMap.AddBlockedField(new Point(7, 5));
        secondMap.AddBlockedField(new Point(7, 6));
        secondMap.AddBlockedField(new Point(7, 7));
        secondMap.AddBlockedField(new Point(7, 9));
        secondMap.AddBlockedField(new Point(0, 7));
        secondMap.AddBlockedField(new Point(1, 7));
        secondMap.AddBlockedField(new Point(2, 7));
        secondMap.AddBlockedField(new Point(3, 7));
        secondMap.AddBlockedField(new Point(4, 7));
        secondMap.AddBlockedField(new Point(5, 7));
        secondMap.AddBlockedField(new Point(6, 7));

        secondMap.AddUnlockedField(new Point(7, 4), 2, "41");
        secondMap.AddKey(new Point(8, 0), 2);

        secondMap.AddUnlockedField(new Point(7, 8), 3, "11");
        secondMap.AddKey(new Point(5, 5), 3);


        // Dodaj pole teleportacyjne na BigMap
        var teleportToSecond = new TeleportField(new Point(9, 0), "SecondMap", new Point(0, 0));
        teleportToSecond.InitMapAndPosition(bigMap, new Point(9, 0));


        var npc2 = new Npc("RandomGuy", "Weź ten klucz jak chcesz przejść dalej. Hasło to suma kwadratów 4 i 5", 'R');
        npc2.InitMapAndPosition(secondMap, new Point(9, 0));

        var filip = new Npc("Filip", "Co wam tak długo zeszło?", 'F');
        filip.InitMapAndPosition(secondMap, new Point(0, 9));

        var wiktor = new Npc("Wiktor", "No w końcu jesteście!", 'W');
        wiktor.InitMapAndPosition(secondMap, new Point(1, 9));

        var npc1 = new Npc("RandomGirl", "Aby przejść dalej musicie ", 'r');
        npc1.InitMapAndPosition(secondMap, new Point(6, 5));

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