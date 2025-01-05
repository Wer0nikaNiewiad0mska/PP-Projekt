using Simulation.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public class Npc : Creature
{
    // Dialog NPC
    public string Dialogue { get; private set; }

    // Symbol NPC
    public override char Symbol { get; protected set; }

    // Konstruktor
    public Npc(string name, string dialogue, char symbol = 'O') : base(name)
    {
        Dialogue = string.IsNullOrWhiteSpace(dialogue) ? "Elo elo 320" : dialogue; // Domyślny dialog
        Symbol = symbol;
    }

    // Sprawdza, czy gracz jest w promieniu jednego pola od NPC i zwraca dialog.
    public string CheckAndSpeak(Point playerPosition)
    {
        // Wyliczanie dystansu
        int dx = Math.Abs(playerPosition.X - Position.X);
        int dy = Math.Abs(playerPosition.Y - Position.Y);

        // Jeśli gracz jest w promieniu, zwraca dialog.
        if (dx <= 1 && dy <= 1)
        {
            return $"{Name}: {Dialogue}";
        }

        return string.Empty; // Brak dialogu, jeśli gracz nie jest w pobliżu.
    }
    public string Speak()
    {
        return $"{Name}: {Dialogue}";
    }
    public void InitMapAndPosition(Map map, Point point)
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map), "Mapa nie może być pusta.");

        if (!map.Exist(point))
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie znajduje się na mapie.");

        Position = point;
        map.Add(this, point); // Dodanie NPC do mapy
        Console.WriteLine($"NPC {Name} został umieszczony na pozycji {point}.");
    }
}
