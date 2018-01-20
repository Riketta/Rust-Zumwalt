
namespace Fougerite
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public class Entity
    {
        public readonly bool hasInventory;
        private readonly object _obj;
        private readonly EntityInv inv;
        private readonly ulong _ownerid;
        private readonly ulong _creatorid;
        private readonly string _creatorname;
        private readonly string _name;
        private readonly string _ownername;
        public bool IsDestroyed = false;

        public Entity(object Obj)
        {
            this._obj = Obj;
            if (Obj is StructureMaster)
            {
                this._ownerid = (Obj as StructureMaster).ownerID;
                this._creatorid = (Obj as StructureMaster).creatorID;
                this._name = "Structure Master";
            }

            if (Obj is StructureComponent)
            {
                this._ownerid = (Obj as StructureComponent)._master.ownerID;
                this._creatorid = (Obj as StructureComponent)._master.creatorID;
                string clone = this.GetObject<StructureComponent>().ToString();
                var index = clone.IndexOf("(Clone)");
                this._name = clone.Substring(0, index);
            }
            if (Obj is DeployableObject)
            {
                this._ownerid = (Obj as DeployableObject).ownerID;
                this._creatorid = (Obj as DeployableObject).creatorID;
                string clone = this.GetObject<DeployableObject>().ToString();
                if (clone.Contains("Barricade"))
                {
                    this._name = "Wood Barricade";
                }
                else
                {
                    var index = clone.IndexOf("(Clone)");
                    this._name = clone.Substring(0, index);
                }
                var deployable = Obj as DeployableObject;

                var inventory = deployable.GetComponent<Inventory>();
                if (inventory != null)
                {
                    this.hasInventory = true;
                    this.inv = new EntityInv(inventory, this);
                }
                else
                {
                    this.hasInventory = false;
                }
            }
            else if (Obj is LootableObject)
            {
                this._ownerid = 76561198095992578UL;
                this._creatorid = 76561198095992578UL;
                var loot = Obj as LootableObject;
                this._name = loot.name;
                var inventory = loot._inventory;
                if (inventory != null)
                {
                    this.hasInventory = true;
                    this.inv = new EntityInv(inventory, this);
                }
                else
                {
                    this.hasInventory = false;
                }
            }
            else if (Obj is SupplyCrate)
            {
                this._ownerid = 76561198095992578UL;
                this._creatorid = 76561198095992578UL;
                this._name = "Supply Crate";
                var crate = Obj as SupplyCrate;
                var inventory = crate.lootableObject._inventory;
                if (inventory != null)
                {
                    this.hasInventory = true;
                    this.inv = new EntityInv(inventory, this);
                }
                else
                {
                    this.hasInventory = false;
                }
            }
            else if (Obj is ResourceTarget)
            {
                var x = (ResourceTarget) Obj;
                this._ownerid = 76561198095992578UL;
                this._creatorid = 76561198095992578UL;
                this._name = x.name;
                this.hasInventory = false;
            }
            else
            {
                this.hasInventory = false;
            }
            if (Fougerite.Server.Cache.ContainsKey(_ownerid))
            {
                this._ownername = Fougerite.Server.Cache[_ownerid].Name;
            }
            else if (Server.GetServer().HasRustPP)
            {
                if (Server.GetServer().GetRustPPAPI().Cache.ContainsKey(_ownerid))
                {
                    this._ownername = Server.GetServer().GetRustPPAPI().Cache[_ownerid];
                }
            }
            else
            {
                this._ownername = "UnKnown";
            }
            if (Fougerite.Server.Cache.ContainsKey(_creatorid))
            {
                this._creatorname = Fougerite.Server.Cache[_creatorid].Name;
            }
            else if (Server.GetServer().HasRustPP)
            {
                if (Server.GetServer().GetRustPPAPI().Cache.ContainsKey(_creatorid))
                {
                    this._creatorname = Server.GetServer().GetRustPPAPI().Cache[_creatorid];
                }
            }
            else
            {
                this._creatorname = "UnKnown";
            }
        }

        public void ChangeOwner(Fougerite.Player p)
        {
            if (this.IsDeployableObject() && !(bool)(this.Object as DeployableObject).GetComponent<SleepingAvatar>())
                this.GetObject<DeployableObject>().SetupCreator(p.PlayerClient.controllable);
            else if (this.IsStructureMaster())
                this.GetObject<StructureMaster>().SetupCreator(p.PlayerClient.controllable);
            else if (this.IsStructure())
            {
                foreach (var st in GetLinkedStructs())
                {
                    if (st.GetObject<StructureMaster>() != null)
                    {
                        this.GetObject<StructureMaster>().SetupCreator(p.PlayerClient.controllable);
                        break;
                    }
                }
            }
        }

        public void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }
            if (this.IsDeployableObject())
            {
                try
                {
                    this.GetObject<DeployableObject>().OnKilled();
                } catch
                {
                    TryNetCullDestroy();
                }
            } else if (this.IsStructure())
            {
                DestroyStructure(this.GetObject<StructureComponent>());                
            } else if (this.IsStructureMaster())
            {
                HashSet<StructureComponent> components = this.GetObject<StructureMaster>()._structureComponents;
                foreach (StructureComponent comp in components)
                    DestroyStructure(comp);

                try 
                {
                    this.GetObject<StructureMaster>().OnDestroy();
                } catch
                {
                    TryNetCullDestroy();
                }
            }
            IsDestroyed = true;
        }

        private void TryNetCullDestroy()
        {
            try
            {
                if (this.IsDeployableObject())
                {
                    if (this.GetObject<DeployableObject>() != null) NetCull.Destroy(this.GetObject<DeployableObject>().networkViewID);
                }
                else if (this.IsStructureMaster())
                {
                    if (this.GetObject<StructureMaster>() != null) NetCull.Destroy(this.GetObject<StructureMaster>().networkViewID);
                }
            }
            catch { }
        }

        private static void DestroyStructure(StructureComponent comp)
        {
            try
            {
                comp._master.RemoveComponent(comp);
                comp._master = null;
                comp.StartCoroutine("DelayedKill");
            } catch
            {
                NetCull.Destroy(comp.networkViewID);
            }
        }

        public List<Entity> GetLinkedStructs()
        {
            List<Entity> list = new List<Entity>();
            var obj = this.Object as StructureComponent;
            if (obj == null)
            {
                list.Add(this);
                return list;
            }
            foreach (StructureComponent component in obj._master._structureComponents)
            {
                if (component != this.Object as StructureComponent)
                {
                    list.Add(new Entity(component));
                }
            }
            return list;
        }

        public T GetObject<T>()
        {
            return (T)this.Object;
        }

        public TakeDamage GetTakeDamage()
        {
            if (this.IsDeployableObject())
            {
                return this.GetObject<DeployableObject>().GetComponent<TakeDamage>();
            }
            if (this.IsStructure())
            {
                return this.GetObject<StructureComponent>().GetComponent<TakeDamage>();
            }
            return null;
        }

        public ResourceTarget ResourceTarget
        {
            get
            {
                if (IsResourceTarget())
                {
                    var x = (ResourceTarget) _obj;
                    return x;
                }
                return null;
            }
        }

        public bool IsResourceTarget()
        {
            return (this.Object is ResourceTarget);
        }

        public bool IsDeployableObject()
        {
            return (this.Object is DeployableObject);
        }

        public bool IsStorage()
        {
            if (this.IsDeployableObject())
                return this.GetObject<DeployableObject>().GetComponent<SaveableInventory>() != null;

            return false;
        }

        public bool IsStructure()
        {
            return (this.Object is StructureComponent);
        }

        public bool IsStructureMaster()
        {
            return (this.Object is StructureMaster);
        }

        public bool IsSleeper()
        {
            if (this.IsDeployableObject())
                return this.GetObject<DeployableObject>().GetComponent<SleepingAvatar>() != null;

            return false;
        }

        public bool IsFireBarrel()
        {
            if (this.IsDeployableObject())
                return this.GetObject<DeployableObject>().GetComponent<FireBarrel>() != null;

            return false;
        }

        public bool IsSupplyCrate()
        {
            return (this.Object is SupplyCrate);
        }

        public void SetDecayEnabled(bool c)
        {
            if (this.IsDeployableObject())
            {
                this.GetObject<DeployableObject>().SetDecayEnabled(c);
            }
        }

        public void UpdateHealth()
        {
            if (this.IsDeployableObject())
            {
                this.GetObject<DeployableObject>().UpdateClientHealth();
            }
            else if (this.IsStructure())
            {
                this.GetObject<StructureComponent>().UpdateClientHealth();
            }
        }

        public Fougerite.Player Creator
        {
            get
            {
                return Fougerite.Server.Cache.ContainsKey(_ownerid) ? Fougerite.Server.Cache[_ownerid] : Fougerite.Player.FindByGameID(this.CreatorID);
            }
        }

        public string OwnerName
        {
            get { return _ownername; }
        }

        public string CreatorName
        {
            get { return _creatorname; }
        }

        public string OwnerID
        {
            get
            {
                return this._ownerid.ToString();
            }
        }

        public ulong UOwnerID
        {
            get
            {
                return this._ownerid;
            }
        }

        public string CreatorID
        {
            get
            {
                return this._creatorid.ToString();
            }
        }

        public float Health
        {
            get
            {
                if (this.IsDeployableObject())
                {
                    return this.GetObject<DeployableObject>().GetComponent<TakeDamage>().health;
                }
                if (this.IsStructure())
                {
                    return this.GetObject<StructureComponent>().GetComponent<TakeDamage>().health;
                }
                if (this.IsStructureMaster())
                {
                    float sum = this.GetObject<StructureMaster>()._structureComponents.Sum<StructureComponent>(s => s.GetComponent<TakeDamage>().health);
                    return sum;
                }
                return 0f;
            }
            set
            {
                if (this.IsDeployableObject())
                {
                    this.GetObject<DeployableObject>().GetComponent<TakeDamage>().health = value;
                }
                else if (this.IsStructure())
                {
                    this.GetObject<StructureComponent>().GetComponent<TakeDamage>().health = value;
                }
                this.UpdateHealth();
            }
        }

        public float MaxHealth
        {
            get
            {
                if (this.IsDeployableObject())
                {
                    return this.GetObject<DeployableObject>().GetComponent<TakeDamage>().maxHealth;
                }
                if (this.IsStructure())
                {
                    return this.GetObject<StructureComponent>().GetComponent<TakeDamage>().maxHealth;
                }
                if (this.IsStructureMaster())
                {
                    float sum = this.GetObject<StructureMaster>()._structureComponents.Sum<StructureComponent>(s => s.GetComponent<TakeDamage>().maxHealth);
                    return sum;
                }
                return 0f;
            }
        }

        public int InstanceID
        {
            get
            {
                if (this.IsDeployableObject())
                {
                    return this.GetObject<DeployableObject>().GetInstanceID();
                }
                if (this.IsStructure())
                {
                    return this.GetObject<StructureComponent>().GetInstanceID();
                }
                return 0;
            }
        }

        public EntityInv Inventory
        {
            get
            {
                if (this.hasInventory)
                    return this.inv;
                return null;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public object Object
        {
            get
            {
                return this._obj;
            }
        }

        public Fougerite.Player Owner
        {
            get
            {
                return Fougerite.Player.FindByGameID(this.OwnerID);
            }
        }

        public Vector3 Location
        {
            get
            {
                if (this.IsDeployableObject())
                    return this.GetObject<DeployableObject>().transform.position;

                if (this.IsStructure())
                    return this.GetObject<StructureComponent>().transform.position;

                if (this.IsStructureMaster())
                    return this.GetObject<StructureMaster>().containedBounds.center;

                return Vector3.zero;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                if (this.IsDeployableObject())
                    return this.GetObject<DeployableObject>().transform.rotation;

                if (this.IsStructure())
                    return this.GetObject<StructureComponent>().transform.rotation;

                return new Quaternion(0, 0, 0, 0);
            }
        }

        public float X
        {
            get
            {
                return this.Location.x;
            }
        }

        public float Y
        {
            get
            {
                return this.Location.y;
            }

        }

        public float Z
        {
            get
            {
                return this.Location.z;
            }
        }
    }
}
