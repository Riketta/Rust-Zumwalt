using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public class BowShootEvent
    {
        private readonly BowWeaponDataBlock _bw;
        private readonly Fougerite.Player _player;
        private readonly ItemRepresentation _ir;
        private readonly uLink.NetworkMessageInfo _unmi;
        private readonly IBowWeaponItem _ibw;

        public BowShootEvent(BowWeaponDataBlock bw, ItemRepresentation ir, uLink.NetworkMessageInfo ui, IBowWeaponItem ibw)
        {
            TakeDamage local = ibw.inventory.GetLocal<TakeDamage>();
            _player = Fougerite.Server.Cache[local.GetComponent<Character>().netUser.userID];
            _bw = bw;
            _ibw = ibw;
            _ir = ir;
            _unmi = ui;
        }

        public void RemoveArrow()
        {
            IBowWeaponItem.RemoveArrowInFlight();
        }

        public IBowWeaponItem IBowWeaponItem
        {
            get { return this._ibw; }
        }

        public Fougerite.Player Player
        {
            get { return this._player; }
        }

        public BowWeaponDataBlock BowWeaponDataBlock
        {
            get { return this._bw; }
        }

        public ItemRepresentation ItemRepresentation
        {
            get { return this._ir; }
        }

        public uLink.NetworkMessageInfo NetworkMessageInfo
        {
            get { return this._unmi; }
        }
    }
}
