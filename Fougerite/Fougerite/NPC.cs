﻿
namespace Fougerite
{
    using UnityEngine;

    public class NPC
    {
        private Character _char;

        public NPC(Character c)
        {
            this._char = c;
        }

        public void Kill()
        {
            this.Character.Signal_ServerCharacterDeath();
            this.Character.SendMessage("OnKilled", new DamageEvent(), SendMessageOptions.DontRequireReceiver);
        }

        public Character Character
        {
            get
            {
                return this._char;
            }
            set
            {
                this._char = value;
            }
        }

        public float Health
        {
            get
            {
                return this._char.health;
            }
            set
            {
                this._char.takeDamage.health = value;
            }
        }

        public string Name
        {
            get
            {
                return this._char.name.Contains("_A(Clone)") ? this._char.name.Replace("_A(Clone)", "") : this._char.name.Replace("(Clone)", "");
            }
        }

        public Vector3 Location
        {
            get { return this._char.transform.position; }
        }

        public float X
        {
            get { return this._char.transform.position.x; }
        }

        public float Y
        {
            get { return this._char.transform.position.y; }
        }

        public float Z
        {
            get { return this._char.transform.position.z; }
        }
    }
}