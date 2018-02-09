using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public class RepairEvent
    {
        private readonly Inventory _inv;
        private readonly RepairBench _rb;
        private readonly Fougerite.Player _pl;
        internal bool _cancel;

        public RepairEvent(RepairBench rb, Inventory inv)
        {
            _rb = rb;
            _inv = inv;
            var netUser = _inv.GetComponent<Character>().netUser;
            if (netUser != null)
            {
                _pl = Fougerite.Server.Cache[netUser.userID];
            }
        }

        public Fougerite.Player Player
        {
            get
            {
                return _pl;
            }
        }

        public Inventory Inv
        {
            get
            {
                return _inv;
            }
        }

        public RepairBench RepairBench
        {
            get
            {
                return _rb;
            }
        }

        public void Cancel()
        {
            if (_cancel)
            {
                return;
            }
            _cancel = true;
        }
    }
}
