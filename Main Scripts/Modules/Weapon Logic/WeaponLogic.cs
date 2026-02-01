using UnityEngine;

namespace UniversalRangedWeaponSystem
{    
    public abstract class WeaponLogic : MonoBehaviour
    {
        [Tooltip("The time (in seconds) before the weapon can be reloaded or fired again.")]
        public float minimumTimeBetweenShots = 0.1f;


        protected Timer firingTimer;


        private void Start()
        {
            firingTimer = new Timer(minimumTimeBetweenShots);
            firingTimer.CurrentTime = 0f;
        }

        private void FixedUpdate()
        {
            if (firingTimer.CurrentTime > 0)
                firingTimer.CurrentTime = Mathf.Clamp(firingTimer.CurrentTime - Time.deltaTime, 0, float.MaxValue);

            // Allow changes during runtime / Also initialises timers
            if (firingTimer.maxTime != minimumTimeBetweenShots)
                firingTimer = new Timer(minimumTimeBetweenShots);
        }

        public virtual void Trigger(RangedWeapon weapon)
        {            
            int ammoSpent = 0;
            // Multi fire w/ consume ammo - decrement for each barrel
            if (weapon.multiBarrelExtension)
            {
                ammoSpent = 1 + weapon.multiBarrelExtension.GetNumberOfAdditionalAmmoToConsume();
            }

            // Regular fire
            else
            {
                ammoSpent = 1;                
            }

            firingTimer.CurrentTime = firingTimer.maxTime;
            weapon.HandleFire(ammoSpent);                 
        }

        public virtual bool ReleaseTriggerOnFire()
        {
            return true;
        }

        public virtual bool IsFiring()
        {            
            return firingTimer.CurrentTime > 0;
        }

        private void Reset()
        {
            var modules = GetComponents<WeaponLogic>();
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
