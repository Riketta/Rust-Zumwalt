﻿namespace Fougerite.Events
{
    using Fougerite;
    using System;

    public class HurtEvent
    {
        private object _attacker;
        private DamageEvent _de;
        private Fougerite.Entity _ent;
        private object _victim;
        private string _weapon;
        private WeaponImpact _wi;

        public HurtEvent(ref DamageEvent d)
        {
            Fougerite.Player player = Fougerite.Player.FindByPlayerClient(d.attacker.client);
            if (player != null)
            {
                this.Attacker = player;
            }
            else
            {
                this.Attacker = new NPC(d.attacker.character);
            }
            Fougerite.Player player2 = Fougerite.Player.FindByPlayerClient(d.victim.client);
            if (player2 != null)
            {
                this.Victim = player2;
            }
            else
            {
                this.Victim = new NPC(d.victim.character);
            }
            this.DamageEvent = d;
            this.WeaponData = null;
            if (d.extraData != null)
            {
                WeaponImpact extraData = d.extraData as WeaponImpact;
                this.WeaponData = extraData;
                string name = "";
                if (extraData.dataBlock != null)
                {
                    name = extraData.dataBlock.name;
                }
                this.WeaponName = name;
            }
        }

        public HurtEvent(ref DamageEvent d, Fougerite.Entity en)
            : this(ref d)
        {
            this.Entity = en;
        }

        public object Attacker
        {
            get
            {
                return this._attacker;
            }
            set
            {
                this._attacker = value;
            }
        }

        public float DamageAmount
        {
            get
            {
                return this._de.amount;
            }
            set
            {
                this._de.amount = value;
            }
        }

        public DamageEvent DamageEvent
        {
            get
            {
                return this._de;
            }
            set
            {
                this._de = value;
            }
        }

        public string DamageType
        {
            get
            {
                string str = "Unknown";
                switch (((int)this.DamageEvent.damageTypes))
                {
                    case 0:
                        return "Bleeding";

                    case 1:
                        return "Generic";

                    case 2:
                        return "Bullet";

                    case 3:
                    case 5:
                    case 6:
                    case 7:
                        return str;

                    case 4:
                        return "Melee";

                    case 8:
                        return "Explosion";

                    case 0x10:
                        return "Radiation";

                    case 0x20:
                        return "Cold";
                }
                return str;
            }
        }

        public Fougerite.Entity Entity
        {
            get
            {
                return this._ent;
            }
            set
            {
                this._ent = value;
            }
        }

        public object Victim
        {
            get
            {
                return this._victim;
            }
            set
            {
                this._victim = value;
            }
        }

        public WeaponImpact WeaponData
        {
            get
            {
                return this._wi;
            }
            set
            {
                this._wi = value;
            }
        }

        public string WeaponName
        {
            get
            {
                return this._weapon;
            }
            set
            {
                this._weapon = value;
            }
        }
    }
}