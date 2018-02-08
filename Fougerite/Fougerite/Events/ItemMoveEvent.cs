using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public class ItemMoveEvent
    {
        private readonly Inventory _from;
        private readonly Inventory _to;
        private readonly int _fslot;
        private readonly int _tslot;
        private readonly Inventory.SlotOperationsInfo _iinfo;
        private readonly Fougerite.Player _pl;

        public ItemMoveEvent(Inventory inst, int fromSlot, Inventory toInventory, int toSlot, Inventory.SlotOperationsInfo info)
        {
            _from = inst;
            _to = toInventory;
            _fslot = fromSlot;
            _tslot = toSlot;
            _iinfo = info;
            foreach (uLink.NetworkPlayer netplayer in toInventory._netListeners)
            {
                NetUser user = netplayer.GetLocalData() as NetUser;
                if (user != null)
                {
                    if (Fougerite.Server.Cache.ContainsKey(user.userID))
                    {
                        _pl = Fougerite.Server.Cache[user.userID];
                    }
                    break;
                }
            }
        }

        public Fougerite.Player Player
        {
            get { return _pl; }
        }

        public Inventory FromInventory
        {
            get{ return _from; }
        }

        public Inventory ToInventory
        {
            get { return _to; }
        }

        public int FromSlot
        {
            get { return _fslot; }
        }

        public int ToSlot
        {
            get { return _tslot; }
        }

        public Inventory.SlotOperationsInfo SlotOperation
        {
            get { return _iinfo; }
        }
    }
}
