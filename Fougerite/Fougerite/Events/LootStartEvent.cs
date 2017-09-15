using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;

namespace Fougerite.Events
{
    public class LootStartEvent
    {
        private bool _cancel;
        private readonly Fougerite.Player _player;
        private readonly LootableObject _lo;
        private readonly Useable _ue;
        private readonly Entity _entity;
        private readonly bool _isobject;
        private readonly uLink.NetworkPlayer _np;
        private Timer _delayer = null;

        public LootStartEvent(LootableObject lo, Fougerite.Player player, Useable use, uLink.NetworkPlayer nplayer)
        {
            _lo = lo;
            _ue = use;
            _player = player;
            _np = nplayer;
            foreach (Collider collider in Physics.OverlapSphere(lo._inventory.transform.position, 1.2f))
            {
                if (collider.GetComponent<DeployableObject>() != null)
                {
                    _entity = new Entity(collider.GetComponent<DeployableObject>());
                    _isobject = true;
                    break;
                }
                if (collider.GetComponent<LootableObject>() != null)
                {
                    _entity = new Entity(collider.GetComponent<LootableObject>());
                    _isobject = false;
                    break;
                }
            }
        }

        internal void LootCancelTimer(object sender, ElapsedEventArgs e)
        {
            _delayer.Dispose();
            // Change callstate, and call eject, for a proper nice exit.
            _ue.callState = (FunctionCallState)0;
            _ue.Eject();
        }

        public void Cancel()
        {
            if (!_cancel)
            {
                _cancel = true;
                if (_delayer != null) return;
                // Delay It, Let LootEnter Hook in Rust to finish.
                _delayer = new Timer(130);
                _delayer.Elapsed += LootCancelTimer;
                _delayer.Start();
            }
        }

        public bool IsObject
        {
            get
            {
                return _isobject;
            }
        }

        public Entity Entity
        {
            get
            {
                return _entity;
            }
        }

        public Fougerite.Player Player
        {
            get
            {
                return _player;
            }
        }

        public Useable Useable
        {
            get
            {
                return _ue;
            }
        }

        public LootableObject LootableObject
        {
            get
            {
                return _lo;
            }
        }

        public bool IsCancelled
        {
            get
            {
                return _cancel;
            }
        }

        public string LootName
        {
            get
            {
                return _lo.name;
            }
        }

        public Inventory RustInventory
        {
            get
            {
                return _lo._inventory;
            }
        }

        public string OccupiedText
        {
            get
            {
                return _lo.occupierText;
            }
            set
            {
                _lo.occupierText = value;
            }
        }
    }
}
