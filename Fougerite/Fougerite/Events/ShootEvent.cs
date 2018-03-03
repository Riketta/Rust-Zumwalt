using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public class ShootEvent
    {
        private readonly BulletWeaponDataBlock _bw;
        private readonly Fougerite.Player _player;
        private readonly UnityEngine.GameObject _go;
        private readonly ItemRepresentation _ir;
        private readonly uLink.NetworkMessageInfo _unmi;
        private readonly IBulletWeaponItem _ibw;

        public ShootEvent(BulletWeaponDataBlock bw, UnityEngine.GameObject go, ItemRepresentation ir, uLink.NetworkMessageInfo ui, IBulletWeaponItem ibw)
        {
            TakeDamage local = ibw.inventory.GetLocal<TakeDamage>();
            _player = Fougerite.Server.Cache[local.GetComponent<Character>().netUser.userID];
            _bw = bw;
            _go = go;
            _ir = ir;
            _ibw = ibw;
            _unmi = ui;
        }

        public IBulletWeaponItem IBulletWeaponItem
        {
            get { return this._ibw; }
        }

        public Fougerite.Player Player
        {
            get { return this._player; }
        }

        public BulletWeaponDataBlock BulletWeaponDataBlock
        {
            get { return this._bw; }
        }

        public UnityEngine.GameObject GameObject
        {
            get { return this._go; }
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
