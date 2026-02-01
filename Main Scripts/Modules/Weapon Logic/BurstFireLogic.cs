using UnityEngine;
using UnityEngine.Events;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Weapon Behaviour/Burst Fire", 2)]
    public class BurstFireLogic : WeaponLogic
    {
        [Tooltip("The time (in seconds) between fires DURING the burst.")]
        public float timeBetweenBurstShots = 0.05f;

        [Tooltip("The number of times the weapon is sequentially fired in a burst.")]
        public int fireCount = 3;

        [Tooltip("Whether a fire should consume the full proper amount of ammo. When enabled, each fire of the weapon will consume one bullet, if disabled then only one bullet is used in a burst.")]
        public bool consumeFullAmmo = true;

        [Tooltip("Called after all bullets in the burst have been fired.")]
        public UnityEvent OnBurstFinish;


        private bool isDoingBurstFire = false;

        public override bool ReleaseTriggerOnFire()
        {
            return false;
        }

        public override void Trigger(RangedWeapon Weapon)
        {
            isDoingBurstFire = true;
            BurstTrigger(fireCount, Weapon);

            // Give control of the gun back to the player
            Weapon.OnTriggerUp();            
        }
        
        private void BurstTrigger(int BurstCount, RangedWeapon Weapon)
        {
            if (BurstCount == 0)
            {
                isDoingBurstFire = false;
                return;
            }

            // Put ammo back into the magazine if it shouldn't be consumed
            if (!consumeFullAmmo && BurstCount > 0)
            {
                Weapon.magazine.AddAmmo(1);
            }

            // If weapon runs out of ammo prematurely then end loop
            if (Weapon.magazine.GetAmmoLeftInMagazine() == 0)
            {
                OnBurstFinish.Invoke();
                isDoingBurstFire = false;
                return;
            }

            base.Trigger(Weapon);

            // Trigger event on final bullet before the wait
            if (BurstCount == fireCount - 1)
                OnBurstFinish.Invoke();

            // Wait for next bullet
            AdditionalFunctions.DelayForSeconds(() => BurstTrigger(BurstCount - 1, Weapon), timeBetweenBurstShots);            
        }

        public override bool IsFiring()
        {
            return base.IsFiring() || isDoingBurstFire;
        }
    }
}
