using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public class GrenadeThrowEvent
    {
        private readonly HandGrenadeDataBlock _bw;
        private readonly Fougerite.Player _player;
        private readonly UnityEngine.GameObject _go;
        private readonly ItemRepresentation _ir;
        private readonly uLink.NetworkMessageInfo _unmi;
        private readonly IHandGrenadeItem _ibw;

        public GrenadeThrowEvent(HandGrenadeDataBlock bw, UnityEngine.GameObject go, ItemRepresentation ir, uLink.NetworkMessageInfo ui, IHandGrenadeItem ibw)
        {
            TakeDamage local = ibw.inventory.GetLocal<TakeDamage>();
            _player = Fougerite.Server.Cache[local.GetComponent<Character>().netUser.userID];
            _bw = bw;
            _go = go;
            _ir = ir;
            _ibw = ibw;
            _unmi = ui;
        }

        public IHandGrenadeItem IHandGrenadeItem
        {
            get { return this._ibw; }
        }

        public Fougerite.Player Player
        {
            get { return this._player; }
        }

        public HandGrenadeDataBlock HandGrenadeDataBlock
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
