using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Maps;

public abstract class Map
{
    private readonly Rectangle _map;
    private readonly Dictionary<Point, List<IMappable>> _fields;
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
        if (!_fields.ContainsKey(position)) return;

        if (_fields[position].Remove(mappable))
        {
            Console.WriteLine($"{mappable.GetType().Name} removed from {position}.");
        }

        if (_fields[position].Count == 0)
        {
            _fields.Remove(position);
        }
    }

    public List<IMappable> At(int x, int y) => At(new Point(x, y));

    public List<IMappable> At(Point p)
    {
        if (_fields.ContainsKey(p)) { return _fields[p]; }
        return new List<IMappable>();
    }

    public void Move(IMappable mappable, Point p, Point pn)
    {
        Remove(mappable, p);
        Add(mappable, pn);
    }

    public virtual bool Exist(Point p) => _map.Contains(p);

    public abstract Point Next(Point p, Direction d);

    public abstract Point NextDiagonal(Point p, Direction d);
    public abstract bool IsBlocked(Point position);

    public abstract bool IsUnlockable(Point position);
}
