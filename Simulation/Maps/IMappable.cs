using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Maps;

public interface IMappable
{
    void InitMapAndPosition(Map map, Point point);
    public abstract char Symbol { get; }
    Point Position { get; }
}
