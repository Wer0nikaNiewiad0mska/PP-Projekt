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
        bigMap.AddBlockedField(new Point(1, 1));
        bigMap.AddPotion(new Point(5, 5), "DoubleMovement");
        bigMap.AddUnlockedField(new Point(7, 9), 1, "1111");
        bigMap.AddKey(new Point(2, 2), 1);
        // Dodaj followera
        var triggerPoint = new Point(9, 0); // Punkt, którego odblokowanie aktywuje followera
        var follower = new Follower("Wercia", triggerPoint);
        bigMap.Add(follower, new Point(9, 1)); // Początkowa pozycja followera
        follower.InitMapAndPosition(bigMap, new Point(9, 1));

        // Dodaj triggerujące pole na mapie
        bigMap.AddTriggerPoint(triggerPoint);


        //Dodanie NPC na mapie
        var Olamo = new Npc("Olamo", "Hejeczka", 'O');
        Olamo.InitMapAndPosition(bigMap, new Point(4,4));

        var npc2 = new Npc("RandomGuy", "Miau miau", 'R');
        npc2.InitMapAndPosition(secondMap, new Point(9,9));

        // Dodaj pole teleportacyjne na BigMap
        var teleportToSecond = new TeleportField(new Point(9, 9), "SecondMap", new Point(0, 0));
        teleportToSecond.InitMapAndPosition(bigMap, new Point(9, 9));

        // Dodaj pole teleportacyjne na SecondMap
        var teleportToBig = new TeleportField(new Point(4, 4), "BigMap", new Point(2, 2));
        teleportToBig.InitMapAndPosition(secondMap, new Point(4, 4));

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