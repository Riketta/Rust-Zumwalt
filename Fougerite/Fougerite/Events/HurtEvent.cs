﻿namespace Fougerite.Events
{
    using Fougerite;
    using System;
    using System.Collections.Generic;

    public class HurtEvent
    {
        private object _attacker;
        private DamageEvent _de;
        private bool _decay;
        private Fougerite.Entity _ent;
        private object _victim;
        private string _weapon;
        private WeaponImpact _wi;
        private readonly bool _playerattacker;
        private readonly bool _playervictim;
        private bool _sleeper;
        private LifeStatus _status;

        public HurtEvent(ref DamageEvent d)
        {
            //Logger.LogDebug(string.Format("[DamageEvent] {0}", d.ToString()));
            try
            {
                this._sleeper = false;
                this.DamageEvent = d;
                this.WeaponData = null;
                this.IsDecay = false;
                this._status = d.status;
                string weaponName = "Unknown";
                if (d.victim.idMain is DeployableObject)
                {
                    if (d.victim.id.ToString().ToLower().Contains("sleeping"))
                    {
                        this._sleeper = true;
                        DeployableObject sleeper = d.victim.idMain as DeployableObject;
                        this.Victim = new Sleeper(sleeper);
                    }
                    else
                    {
                        this.Victim = new Entity(d.victim.idMain.GetComponent<DeployableObject>());
                    }
                    this._playervictim = false;
                }
                else if (d.victim.idMain is StructureComponent)
                {
                    this.Victim = new Entity(d.victim.idMain.GetComponent<StructureComponent>());
                    this._playervictim = false;
                }
                else if (d.victim.id is SpikeWall)
                {
                    this._playerattacker = false;
                    this.Victim = new Entity(d.victim.idMain.GetComponent<DeployableObject>());
                }
                else if (d.victim.client != null)
                {
                    this.Victim = Fougerite.Server.Cache[d.victim.client.userID];
                    this._playervictim = true;
                }
                else if (d.victim.character != null)
                {
                    this.Victim = new NPC(d.victim.character);
                    this._playervictim = false;
                }
                if (!(bool) d.attacker.id)
                {
                    if (d.victim.client != null)
                    {
                        weaponName = this.DamageType;
                        this._playerattacker = false;
                        this.Attacker = null;
                    }
                }
                else if (d.attacker.id is SpikeWall)
                {
                    this._playerattacker = false;
                    this.Attacker = new Entity(d.attacker.idMain.GetComponent<DeployableObject>());
                    weaponName = d.attacker.id.ToString().Contains("Large") ? "Large Spike Wall" : "Spike Wall";
                }
                else if (d.attacker.id is SupplyCrate)
                {
                    this._playerattacker = false;
                    this.Attacker = new Entity(d.attacker.idMain.gameObject);
                    weaponName = "Supply Crate";
                }
                else if (d.attacker.id is Metabolism && d.victim.id is Metabolism)
                {
                    //this.Attacker = Fougerite.Server.Cache[d.attacker.client.userID];
                    this.Attacker = Fougerite.Player.FindByPlayerClient(d.attacker.client);
                    this._playerattacker = false;
                    this.Victim = this.Attacker;
                    ICollection<string> list = new List<string>();
                    Fougerite.Player vic = this.Victim as Fougerite.Player;
                    if (vic.IsStarving)
                    {
                        list.Add("Starvation");
                    }
                    if (vic.IsRadPoisoned)
                    {
                        list.Add("Radiation");
                        /*if (vic.RadLevel > 5000)
                        {
                            vic.AddRads(-vic.RadLevel);
                            d.amount = 0;
                            _de.amount = 0;
                            Logger.LogDebug("[RadiationHack] Someone tried to kill " + vic.Name + " with radiation hacks.");
                        }*/
                    }
                    if (vic.IsPoisoned)
                    {
                        list.Add("Poison");
                    }
                    if (vic.IsBleeding)
                    {
                        list.Add("Bleeding");
                    }

                    if (list.Contains("Bleeding"))
                    {
                        if (this.DamageType != "Unknown" && !list.Contains(this.DamageType))
                            list.Add(this.DamageType);
                    }
                    if (list.Count > 0)
                    {
                        weaponName = string.Format("Self ({0})", string.Join(",", list.ToArray()));
                    }
                    else
                    {
                        weaponName = this.DamageType;
                    }
                }
                else if (d.attacker.client != null)
                {
                    this.Attacker = Fougerite.Server.Cache[d.attacker.client.userID];
                    this._playerattacker = true;
                    if (d.extraData != null)
                    {
                        WeaponImpact extraData = d.extraData as WeaponImpact;
                        this.WeaponData = extraData;
                        if (extraData.dataBlock != null)
                        {
                            weaponName = extraData.dataBlock.name;
                        }
                    }
                    else
                    {
                        if (d.attacker.id is TimedExplosive)
                        {
                            weaponName = "Explosive Charge";
                        }
                        else if (d.attacker.id is TimedGrenade)
                        {
                            weaponName = "F1 Grenade";
                        }
                        else
                        {
                            weaponName = "Hunting Bow";
                        }
                        if (d.victim.client != null)
                        {
                            if (!d.attacker.IsDifferentPlayer(d.victim.client) && !(this.Victim is Entity))
                            {
                                weaponName = "Fall Damage";
                            }
                            else if (!d.attacker.IsDifferentPlayer(d.victim.client) && (this.Victim is Entity))
                            {
                                weaponName = "Hunting Bow";
                            }
                        }
                    }
                }
                else if (d.attacker.character != null)
                {
                    this.Attacker = new NPC(d.attacker.character);
                    this._playerattacker = false;
                    weaponName = string.Format("{0} Claw", (this.Attacker as NPC).Name);
                }
                this.WeaponName = weaponName;
            }
            catch (Exception ex)
            {
                Logger.LogDebug(string.Format("[HurtEvent] Error. " + ex.ToString()));
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

        public LifeStatus LifeStatus
        {
            get { return this._status; }
        }

        public bool Sleeper
        {
            get
            {
                return this._sleeper;
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

        public bool IsDecay
        {
            get
            {
                return this._decay;
            }
            set
            {
                this._decay = value;
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

        public bool VictimIsPlayer
        {
            get
            {
                return this._playervictim;
            }       
        }

        public bool AttackerIsPlayer
        {
            get
            {
                return this._playerattacker;
            }
        }
    }
}
