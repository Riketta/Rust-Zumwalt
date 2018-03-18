using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    /// <summary>
    /// This class is created when a shotgun is shot.
    /// </summary>
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

        /// <summary>
        /// The amount of pellets that are going to fly from the gun.
        /// </summary>
        /// <param name="pellets"></param>
        public void SetPellets(int pellets)
        {
            _bw.numPellets = pellets;
        }

        /// <summary>
        /// The IBulletWeaponItem of the gun.
        /// </summary>
        public IBulletWeaponItem IBulletWeaponItem
        {
            get { return this._ibw; }
        }

        /// <summary>
        /// The player who shoots the gun.
        /// </summary>
        public Fougerite.Player Player
        {
            get { return this._player; }
        }

        /// <summary>
        /// The datablock of the shotgun.
        /// </summary>
        public ShotgunDataBlock ShotgunDataBlock
        {
            get { return this._bw; }
        }

        /// <summary>
        /// Gets the ItemRepresentation class
        /// </summary>
        public ItemRepresentation ItemRepresentation
        {
            get { return this._ir; }
        }

        /// <summary>
        /// Gets the uLink.NetworkMessageInfo data.
        /// </summary>
        public uLink.NetworkMessageInfo NetworkMessageInfo
        {
            get { return this._unmi; }
        }
    }
}
