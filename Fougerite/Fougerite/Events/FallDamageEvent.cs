using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public class FallDamageEvent
    {
        private readonly float _fallspeed;
        private readonly float _num;
        private readonly FallDamage _fd;
        private readonly bool _flag;
        private readonly bool _flag2;
        private readonly Fougerite.Player _player;

        public FallDamageEvent(FallDamage fd, float speed, float num, bool flag, bool flag2)
        {
            _fd = fd;
            _player = Fougerite.Server.Cache[fd.idMain.netUser.userID];
            _fallspeed = speed;
            _num = num;
            _flag = flag;
            _flag2 = flag2;
        }

        public Fougerite.Player Player
        {
            get { return _player; }
        }

        public float FloatSpeed
        {
            get { return _fallspeed; }
        }

        public float Num
        {
            get { return _num; }
        }

        public FallDamage FallDamage
        {
            get { return _fd; }
        }

        public bool Bleeding
        {
            get { return _flag; }
        }

        public bool BrokenLegs
        {
            get { return _flag2; }
        }

        public void Cancel()
        {
            if (_player.IsOnline)
            {
                if (BrokenLegs)
                {
                    _fd.ClearInjury();
                }
                if (Bleeding)
                {
                    _player.HumanBodyTakeDmg.SetBleedingLevel(0f);
                }
            }
        }
    }
}
