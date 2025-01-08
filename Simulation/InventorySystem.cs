using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation;

namespace Simulation;
public class InventorySystem
{
    private const int MAXIMUM_SLOTS_IN_INVENTORY = 10;
    public readonly List<InventoryRecord> InventoryRecords = new List<InventoryRecord>();

    public void AddItem(ObtainableItem item, int quantityToAdd)
    {
        while (quantityToAdd > 0)
        {
            var existingRecord = InventoryRecords.FirstOrDefault(x => x.InventoryItem.ID == item.ID && x.Quantity < x.InventoryItem.MaximumStackableQuantity);
            if (existingRecord != null)
            {
                int maxAddable = existingRecord.InventoryItem.MaximumStackableQuantity - existingRecord.Quantity;
                int addToStack = Math.Min(quantityToAdd, maxAddable);
                existingRecord.AddToQuantity(addToStack);
                quantityToAdd -= addToStack;
            }
            else
            {
                if (InventoryRecords.Count < MAXIMUM_SLOTS_IN_INVENTORY)
                {
                    int initialQuantity = Math.Min(quantityToAdd, item.MaximumStackableQuantity);
                    InventoryRecords.Add(new InventoryRecord(item, initialQuantity));
                    quantityToAdd -= initialQuantity;
                }
                else
                {
                    throw new Exception("No more space in inventory");
                }
            }
        }
    }

    public class InventoryRecord
    {
        public ObtainableItem InventoryItem { get; private set; }
        public int Quantity { get; private set; }

        public InventoryRecord(ObtainableItem item, int quantity)
        {
            InventoryItem = item;
            Quantity = quantity;
        }

        public void AddToQuantity(int amountToAdd)
        {
            Quantity += amountToAdd;
        }
    }
}