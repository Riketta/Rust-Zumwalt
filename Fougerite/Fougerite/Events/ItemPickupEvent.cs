using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Fougerite.Events
{
    /// <summary>
    /// This class is created when an item is picked up.
    /// </summary>
    public class ItemPickupEvent
    {
        private readonly Fougerite.Player _player;
        private readonly IInventoryItem _item;
        private readonly Inventory _inv;
        private bool _cancelled;
        private Timer _delayer = null;

        public ItemPickupEvent(Controllable controllable, IInventoryItem item, Inventory local)
        {
            _player = Fougerite.Server.Cache[controllable.netUser.userID];
            _item = item;
            _inv = local;
        }

        /// <summary>
        /// The player who is picking the item up.
        /// </summary>
        public Fougerite.Player Player
        {
            get { return _player; }
        }

        /// <summary>
        /// The item we are picking up.
        /// </summary>
        public IInventoryItem Item
        {
            get { return _item; }
        }

        /// <summary>
        /// The inventory where the item is going to be placed.
        /// </summary>
        public Inventory Inventory
        {
            get { return _inv; }
        }

        /// <summary>
        /// Is the event cancelled?
        /// </summary>
        public bool Cancelled
        {
            get { return _cancelled; }
        }

        /// <summary>
        /// Cancels the event.
        /// </summary>
        public void Cancel()
        {
            if (_cancelled) return;
            _cancelled = true;
        }
    }
}
