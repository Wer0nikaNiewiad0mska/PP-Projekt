using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Maps;

public abstract class Map
{
    private readonly Rectangle _map;
    private readonly Dictionary<Point, List<IMappable>> _fields = new();
    private readonly HashSet<Point> _triggerPoints = new();
    public int SizeX { get; }
    public int SizeY { get; }
    protected Map(int sizeX, int sizeY)
    {
        if (sizeX < 5) throw new ArgumentOutOfRangeException(nameof(sizeX), "Too narrow");
        if (sizeY < 5) throw new ArgumentOutOfRangeException(nameof(sizeY), "Too short");

        SizeX = sizeX;
        SizeY = sizeY;

        _map = new Rectangle(0, 0, SizeX - 1, SizeY - 1);
        _fields = new Dictionary<Point, List<IMappable>>();
    }

    public void Add(IMappable mappable, Point position)
    {
        if (!Exist(position)) throw new ArgumentOutOfRangeException(nameof(position), "Position is not on the map.");

        if (!_fields.ContainsKey(position)) { _fields[position] = new List<IMappable>(); }
        _fields[position].Add(mappable);
    }

    public void Remove(IMappable mappable, Point position)
    {
        Console.WriteLine($"Próba usunięcia {mappable.GetType().Name} z pozycji {position}.");

        // Sprawdź, czy istnieją obiekty na danej pozycji
        if (!_fields.ContainsKey(position))
        {
            Console.WriteLine($"Brak obiektów na pozycji {position}, nic do usunięcia.");
            return;
        }

        // Usuń konkretny obiekt z listy
        if (_fields[position].Remove(mappable))
        {
            Console.WriteLine($"{mappable.GetType().Name} został usunięty z pozycji {position}.");
        }
        else
        {
            Console.WriteLine($"{mappable.GetType().Name} nie znaleziono na pozycji {position}.");
        }

        // Usuń pustą listę, jeśli brak obiektów na danej pozycji
        if (_fields[position].Count == 0)
        {
            _fields.Remove(position);
            Console.WriteLine($"Pozycja {position} jest teraz pusta i została usunięta z mapy.");
        }
    }
    public bool TryGetField(Point point, out List<IMappable> mappableObjects)
    {
        if (_fields.TryGetValue(point, out var objects))
        {
            mappableObjects = objects;
            return true;
        }

        mappableObjects = null;
        return false;
    }
    public bool IsTeleport(Point position)
    {
        // Sprawdź, czy na pozycji znajduje się teleport
        if (_fields.TryGetValue(position, out var objects))
        {
            return objects.OfType<TeleportField>().Any();
        }
        return false;
    }
    public List<IMappable> At(int x, int y) => At(new Point(x, y));

    public List<IMappable> At(Point position)
    {
        if (_fields.ContainsKey(position))
            return _fields[position];

        return new List<IMappable>(); // Zwraca pustą listę, jeśli brak obiektów
    }

    public void Move(IMappable mappable, Point p, Point pn)
    {
        Remove(mappable, p);
        Add(mappable, pn);
    }

    public void AddTriggerPoint(Point point)
    {
        if (!Exist(point))
            throw new ArgumentOutOfRangeException(nameof(point), "Punkt nie istnieje na mapie.");

        _triggerPoints.Add(point);
    }

    public bool IsTriggerPoint(Point point)
    {
        return _triggerPoints.Contains(point);
    }

    public virtual bool Exist(Point p) => _map.Contains(p);

    public abstract Point Next(Point p, Direction d);

    public abstract bool IsBlocked(Point position);

    public abstract bool IsUnlockable(Point position);
    public abstract bool IsPotion(Point position);
    public abstract bool IsKey(Point position);

    public abstract bool IsNpc(Point position);
}
