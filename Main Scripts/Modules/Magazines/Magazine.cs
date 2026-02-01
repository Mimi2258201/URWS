using UnityEngine;
using UnityEngine.Events;

namespace UniversalRangedWeaponSystem
{    
    public abstract class Magazine : MonoBehaviour
    {
        [Tooltip("The maximum number of bullets that can be held. Zero indicates that there is no limit.")]
        public int maxAmmoCount = 0;

        [Tooltip("Whether the weapon should start with full ammo, else weapon will have no ammo when starting.")]
        public bool startWithAmmo = true;

        
        protected int totalAmmoCount = 10;

        protected virtual void Start()
        {
            if (startWithAmmo)
            {
                if (maxAmmoCount == 0)
                    totalAmmoCount = int.MaxValue;
                else
                    totalAmmoCount = maxAmmoCount;
            }            
        }        

        // Base functions
        public virtual bool IsReloading()
        {
            return false;
        }

        // Virtual functions
        public virtual bool Reload()
        {
            return false;
            
        }

        public virtual string GetAmmo()
        {
            if (maxAmmoCount == 0)
                return "∞";

            return totalAmmoCount.ToString();
        }

        public virtual int GetAmmoLeftInMagazine()
        {
            return totalAmmoCount;
        }

        public virtual void SpendAmmo(int ammoToSpend)
        {
            if (ammoToSpend > totalAmmoCount)
                Debug.LogWarning("Spent more ammo than available. Values may appear negative.");

            if (maxAmmoCount != 0)
                totalAmmoCount -= ammoToSpend;
        }        

        public virtual void AddAmmo(int ammoToAdd)
        {
            totalAmmoCount += ammoToAdd;
        }

        private void Reset()
        {
            var modules = GetComponents<Magazine>();
            if (modules.Length > 1)
            {
                foreach (var module in modules)
                {
                    if (module != this)
                        DestroyImmediate(module);
                }
            }
        }        
    }
}
