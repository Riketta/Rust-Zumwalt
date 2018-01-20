
using System.Timers;

namespace Fougerite.Events
{
    public class InventoryModEvent
    {
        private readonly Inventory _inventory;
        private readonly int _slot;
        private readonly IInventoryItem _item;
        private readonly Fougerite.Player _player = null;
        private readonly NetUser _netuser = null;
        private readonly uLink.NetworkPlayer _netplayer;
        private readonly string _etype;
        private Timer _delayer = null;

        public InventoryModEvent(Inventory inventory, int slot, IInventoryItem item, string type)
        {
            this._inventory = inventory;
            this._slot = slot;
            this._item = item;
            this._etype = type;
            foreach (uLink.NetworkPlayer netplayer in inventory._netListeners)
            {
                NetUser user = netplayer.GetLocalData() as NetUser;
                if (user != null)
                {
                    _netuser = user;
                    if (Fougerite.Server.Cache.ContainsKey(_netuser.userID)) { _player = Fougerite.Server.Cache[_netuser.userID]; }
                    _netplayer = netplayer;
                    break;
                }
            }
        }

        public void Cancel()
        {
            if (_netuser == null) return;
            if (_netuser.playerClient == null) return;
            if (_delayer != null) return;
            // Timer is required, since rust doesn't update the player's inventory immediately.
            _delayer = new Timer(500);
            _delayer.Elapsed += CancelDelay;
            _delayer.Start();
        }

        private void CancelDelay(object sender, ElapsedEventArgs e)
        {
            _delayer.Stop();
            _delayer.Dispose();
            if (_etype == "Add")
            {
                _player.Inventory.AddItem(_item.datablock.name, _item.uses);
                _inventory.RemoveItem(_item);
            }
            else
            {
                _inventory.AddItemAmount(_item.datablock, _item.uses);
                _player.Inventory.RemoveItem(_item.datablock.name, _item.uses);
            }
        }

        public string ItemName
        {
            get { return _item.datablock.name; }
        }

        public Fougerite.Player Player
        {
            get { return _player; }
        }

        public NetUser NetUser
        {
            get { return _netuser; }
        }

        public uLink.NetworkPlayer NetPlayer
        {
            get { return _netplayer; }
        }

        public IInventoryItem InventoryItem
        {
            get { return _item; }
        }

        public EntityItem Item
        {
            get { return new EntityItem(_inventory, _slot); }
        }

        public int Slot
        {
            get { return _slot; }
        }

        public Inventory Inventory
        {
            get { return _inventory; }
        }

        public FInventory FInventory
        {
            get { return new FInventory(_inventory); }
        }

        public string Type
        {
            get { return _etype; }
        }
    }
}
