using System.Linq;

namespace Fougerite
{
	/// <summary>
	/// Represents an Item on a slot.
	/// </summary>
    public class EntityItem
    {
        private readonly Inventory internalInv;
		private readonly int internalSlot;

        public EntityItem(Inventory inv, int slot)
		{
			this.internalInv = inv;
			this.internalSlot = slot;
		}

	    /// <summary>
	    /// Drops this item from the inventory.
	    /// </summary>
		public void Drop()
		{
			DropHelper.DropItem(this.internalInv, this.Slot);
		}

		private IInventoryItem GetItemRef()
		{
			IInventoryItem item;
			this.internalInv.GetItem(this.internalSlot, out item);
			return item;
		}

	    /// <summary>
	    /// Checks if the Item Slot is empty.
	    /// </summary>
	    /// <returns></returns>
		public bool IsEmpty()
		{
			return (this.RInventoryItem == null);
		}

	    /// <summary>
	    /// Gets the original IInventoryItem of this item from the rust api.
	    /// </summary>
		public IInventoryItem RInventoryItem
		{
			get
			{
				return this.GetItemRef();
			}
		}

	    /// <summary>
	    /// Gets / Sets the name of this item.
	    /// </summary>
		public string Name
		{
			get
			{
				if (!this.IsEmpty())
				{
					return this.RInventoryItem.datablock.name;
				}
				return "Empty slot";
			}
			set
			{
				this.RInventoryItem.datablock.name = value;
			}
		}

	    /// <summary>
	    /// Gets the amount of the item in this slot.
	    /// </summary>
		public int Quantity
		{
			get
			{
			    return Util.UStackable.Contains(Name) ? 1 : this.UsesLeft;
			}
		    set
			{
				this.UsesLeft = value;
			}
		}

	    /// <summary>
	    /// Gets the slot of the item.
	    /// </summary>
		public int Slot
		{
			get
			{
				if (!this.IsEmpty())
				{
					return this.RInventoryItem.slot;
				}
				return this.internalSlot;
			}
		}

	    /// <summary>
	    /// Gets the uses remaining of the item. (Ammo, Research kit, etc.)
	    /// </summary>
		public int UsesLeft
		{
			get
			{
				if (!this.IsEmpty())
				{
					return this.RInventoryItem.uses;
				}
				return -1;
			}
			set
			{
				this.RInventoryItem.SetUses(value);
			}
		}
    }
}
