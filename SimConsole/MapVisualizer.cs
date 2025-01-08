using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation;
using Simulation.Maps;

namespace SimConsole;

public class MapVisualizer
{
    private readonly Map _map;

    public MapVisualizer(Map map)
    {
        _map = map ?? throw new ArgumentNullException(nameof(map));
        Console.OutputEncoding = Encoding.UTF8;
    }

    public void Draw()
    {
        // Walidacja wielkości mapy względem rozmiarów okna konsoli
        if (_map.SizeX > Console.WindowWidth || _map.SizeY > Console.WindowHeight)
        {
            throw new InvalidOperationException("Rozmiar mapy przekracza wymiary okna konsoli.");
        }

        // Góra - krawędź
        Console.Write(Box.TopLeft);
        for (int x = 0; x < _map.SizeX - 1; x++)
        {
            Console.Write($"{Box.Horizontal}{Box.TopMid}");
        }
        Console.WriteLine($"{Box.Horizontal}{Box.TopRight}");

        // Środek mapy
        for (int y = _map.SizeY - 1; y >= 0; y--)
        {
            Console.Write(Box.Vertical);
            for (int x = 0; x < _map.SizeX; x++)
            {
                var creatures = _map.At(x, y) ?? new List<IMappable>();

                // Sprawdzanie zawartości pola
                if (_map is BigMap bigMap)
                {
                    if (bigMap.IsBlocked(new Point(x, y)))
                    {
                        Console.Write("X"); // Zablokowane pole
                    }
                    else if (bigMap.IsUnlockable(new Point(x, y)))
                    {
                        Console.Write("Y"); // Pole do odblokowania
                    }
                    else if (bigMap.IsKey(new Point(x, y)))
                    {
                        Console.Write("*"); // Klucz
                    }
                    else if (creatures.Count == 1)
                    {
                        Console.Write(creatures.First().Symbol); // Symbol pojedynczej postaci
                    }
                    else if (creatures.Count > 1)
                    {
                        Console.Write(creatures.Count); // Liczba postaci na polu
                    }
                    else if (bigMap.IsKey(new Point(x, y)))
                    {
                        Console.Write("*"); // Klucz
                    }
                    else
                    {
                        Console.Write(" "); // Puste pole
                    }
                }
                Console.Write(Box.Vertical);
            }
            Console.WriteLine();

            // Krawędzie środkowe
            if (y > 0)
            {
                Console.Write(Box.MidLeft);
                for (int x = 0; x < _map.SizeX - 1; x++)
                {
                    Console.Write($"{Box.Horizontal}{Box.Cross}");
                }
                Console.WriteLine($"{Box.Horizontal}{Box.MidRight}");
            }
        }

        // Dół - krawędź
        Console.Write(Box.BottomLeft);
        for (int x = 0; x < _map.SizeX - 1; x++)
        {
            Console.Write($"{Box.Horizontal}{Box.BottomMid}");
        }
        Console.WriteLine($"{Box.Horizontal}{Box.BottomRight}");
        Console.WriteLine();

    }
}