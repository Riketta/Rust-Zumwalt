﻿
namespace Fougerite
{
    using UnityEngine;
    public class Sleeper
    {
        private DeployableObject _sleeper;
        private ulong _uid;
        private int _instanceid;
        private string _name;
        public bool IsDestroyed = false;

        public Sleeper(DeployableObject obj)
        {
            this._sleeper = obj;
            this._instanceid = this._sleeper.GetInstanceID();
            this._uid = this._sleeper.ownerID;
            this._name = Fougerite.Server.Cache.ContainsKey(UID) ? Fougerite.Server.Cache[UID].Name : this._sleeper.ownerName;
        }

        /// <summary>
        /// Gets the Sleeper's health.
        /// </summary>
        public float Health
        {
            get
            {
                return this._sleeper.GetComponent<TakeDamage>().health;
            }
            set
            {
                this._sleeper.GetComponent<TakeDamage>().health = value;
                this.UpdateHealth();
            }
        }

        public void UpdateHealth()
        {
            this._sleeper.UpdateClientHealth();
        }

        /// <summary>
        /// Destroys the sleeper.
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }
            try
            {
                this._sleeper.OnKilled();
            }
            catch
            {
                TryNetCullDestroy();
            }
            IsDestroyed = true;
        }

        private void TryNetCullDestroy()
        {
            try
            {
                NetCull.Destroy(this._sleeper.networkViewID);
            }
            catch { }
        }

        public DeployableObject Object
        {
            get { return this._sleeper; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string OwnerID
        {
            get { return this._uid.ToString(); }
        }

        public string SteamID
        {
            get { return this._uid.ToString(); }
        }

        public ulong UID
        {
            get { return this._uid; }
        }

        public string OwnerName
        {
            get { return this._sleeper.ownerName; }
        }

        public Vector3 Location
        {
            get { return this._sleeper.transform.position; }
        }

        public float X
        {
            get { return this._sleeper.transform.position.x; }
        }

        public float Y
        {
            get { return this._sleeper.transform.position.y; }
        }

        public float Z
        {
            get { return this._sleeper.transform.position.z; }
        }

        public int InstanceID
        {
            get
            {
                return this._instanceid;
            }
        }
    }
}
