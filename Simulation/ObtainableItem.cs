﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public abstract class ObtainableItem
{
    public string Name { get; private set; }
    public int MaximumStackableQuantity { get; set; }
    public Guid ID { get; private set; } = Guid.NewGuid(); // Unique ID for each item type

    protected ObtainableItem(string name, int maxStackable = 1)
    {
        Name = name;
        MaximumStackableQuantity = maxStackable;
    }
}

public abstract class WorldItem
{
}

public interface IActsOn<in TWorldItem>
    where TWorldItem : WorldItem
{
    void ApplyTo(TWorldItem worldItem);
}

public class World
{
    private Action<ObtainableItem, WorldItem> GetApplyTo(Type tWorldItem, Type tInvItem)
    {
        var tActOn = typeof(IActsOn<>).MakeGenericType(tWorldItem);
        if (!tActOn.IsAssignableFrom(tInvItem))
        {
            return null;
        }
        var methodInfo = tActOn.GetMethod(nameof(IActsOn<WorldItem>.ApplyTo));

        return new Action<ObtainableItem, WorldItem>((invItem, worldItem) =>
        {
            methodInfo.Invoke(invItem, new object[] { worldItem });
        });
    }

    public bool IsDropTarget(WorldItem worldItem, ObtainableItem item)
        => GetApplyTo(worldItem.GetType(), item.GetType()) != null;

    public void ActOn(WorldItem worldItem, ObtainableItem item)
    {
        var actOn = GetApplyTo(worldItem.GetType(), item.GetType());
        if (actOn == null)
        {
            throw new InvalidOperationException();
        }

        actOn(item, worldItem);
    }
}