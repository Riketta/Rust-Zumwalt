using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Fougerite.Events
{
    public class ItemPickupEvent
    {
        private readonly Fougerite.Player _player;
        private readonly IInventoryItem _item;
        private readonly Inventory _inv;
        private readonly Inventory.AddExistingItemResult _result;
        private readonly bool _pickedup;
        private bool _cancelled;
        private Timer _delayer = null;

        public ItemPickupEvent(Controllable controllable, IInventoryItem item, Inventory local, Inventory.AddExistingItemResult result)
        {
            _player = Fougerite.Server.Cache[controllable.netUser.userID];
            _item = item;
            _inv = local;
            _result = result;
            switch (result)
            {
                case Inventory.AddExistingItemResult.CompletlyStacked:
                    _pickedup = true;
                    break;

                case Inventory.AddExistingItemResult.Moved:
                    _pickedup = true;
                    break;

                case Inventory.AddExistingItemResult.PartiallyStacked:
                    _pickedup = true;
                    break;

                case Inventory.AddExistingItemResult.Failed:
                    _pickedup = false;
                    break;

                case Inventory.AddExistingItemResult.BadItemArgument:
                    _pickedup = false;
                    break;

                default:
                    _pickedup = false;
                    break;
            }
        }

        public Fougerite.Player Player
        {
            get { return _player; }
        }

        public IInventoryItem Item
        {
            get { return _item; }
        }

        public Inventory Inventory
        {
            get { return _inv; }
        }

        public Inventory.AddExistingItemResult Result
        {
            get { return _result; }
        }

        public bool PickedkUp
        {
            get { return _pickedup; }
        }

        public bool Cancelled
        {
            get { return _cancelled; }
        }

        public void Cancel()
        {
            if (Cancelled || !_pickedup) { return; }
            // Timer is required, since rust doesn't update the player's inventory immediately.
            _delayer = new Timer(500);
            _delayer.Elapsed += CancelDelay;
            _delayer.Start();
        }

        internal void CancelDelay(object sender, ElapsedEventArgs e)
        {
            _delayer.Stop();
            _delayer.Dispose();
            _cancelled = true;
            Player.Inventory.DropItem(_item.slot);
        }
    }
}
