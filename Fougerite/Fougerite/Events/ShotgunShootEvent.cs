using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public class ShotgunShootEvent
    {
        private readonly ShotgunDataBlock _bw;
        private readonly Fougerite.Player _player;
        private readonly ItemRepresentation _ir;
        private readonly uLink.NetworkMessageInfo _unmi;
        private readonly IBulletWeaponItem _ibw;

        public ShotgunShootEvent(ShotgunDataBlock bw, ItemRepresentation ir, uLink.NetworkMessageInfo ui, IBulletWeaponItem ibw)
        {
            TakeDamage local = ibw.inventory.GetLocal<TakeDamage>();
            _player = Fougerite.Server.Cache[local.GetComponent<Character>().netUser.userID];
            _bw = bw;
            _ir = ir;
            _ibw = ibw;
            _unmi = ui;
        }

        public void SetPellets(int pellets)
        {
            _bw.numPellets = pellets;
        }

        public IBulletWeaponItem IBulletWeaponItem
        {
            get { return this._ibw; }
        }

        public Fougerite.Player Player
        {
            get { return this._player; }
        }

        public ShotgunDataBlock ShotgunDataBlock
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
