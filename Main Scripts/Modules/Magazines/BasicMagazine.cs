using UnityEngine;
using UnityEngine.Events;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Magazines/Basic Magazine", 2)]
    public class BasicMagazine : Magazine
    {
        [Header("Basic Magazine Properties")]
        [Tooltip("The number of bullets in a magazine such that a magazine size of 1 requires a reload after every fire")]
        public int magazineAmmoCapacity = 12;

        [Tooltip("Should the weapon remember the bullets in the current magazine when reloading. Turning this off results in bullets being wasted/lost.")]
        public bool isAmmoPersistent = true;

        [Tooltip("The time (in seconds) that it takes for the reload to take effect and the weapon to be functional again.")]
        public float reloadTimeDuration = 0.5f;

        [Tooltip("Called every time the weapon successfully reloads.")]
        public UnityEvent OnReload;


        protected Timer reloadTimer;

        private int currentMagazineBulletCount = 0;

        protected override void Start()
        {
            if (startWithAmmo)
            {                
                currentMagazineBulletCount = magazineAmmoCapacity;
                totalAmmoCount = maxAmmoCount * magazineAmmoCapacity;
                if (maxAmmoCount == 0)
                    totalAmmoCount = int.MaxValue / 2; // no overflow errors please
            }

            reloadTimer = new Timer(reloadTimeDuration);
            reloadTimer.CurrentTime = 0f;
        }

        private void FixedUpdate()
        {
            if (reloadTimer.CurrentTime > 0)
                reloadTimer.CurrentTime = Mathf.Clamp(reloadTimer.CurrentTime - Time.deltaTime, 0, float.MaxValue);

            if (reloadTimer.maxTime != reloadTimeDuration)
                reloadTimer = new Timer(reloadTimeDuration);
        }

        public override bool Reload()
        {
            if ((totalAmmoCount > 0 || maxAmmoCount == 0) && currentMagazineBulletCount < magazineAmmoCapacity)
            {
                if (isAmmoPersistent)
                // From this point, we assume the reload is (mostly) valid.
                {
                    if (maxAmmoCount == 0)
                        totalAmmoCount += magazineAmmoCapacity;

                    // Can we fill the current clip
                    if (totalAmmoCount >= magazineAmmoCapacity)
                    {
                        currentMagazineBulletCount = magazineAmmoCapacity;
                    }
                    else
                    {
                        // Put everything we can in there which won't fill the clip
                        currentMagazineBulletCount += totalAmmoCount - currentMagazineBulletCount;
                    }
                }
                else
                {
                    // Ammo has more than a magazine, indicates there is at least some ammo to reload in.
                    if (totalAmmoCount > magazineAmmoCapacity)
                    {
                        totalAmmoCount -= currentMagazineBulletCount;
                        currentMagazineBulletCount = magazineAmmoCapacity;

                    }

                    // No spare ammo
                    else
                        return false;

                }
                reloadTimer.CurrentTime = reloadTimer.maxTime;
                OnReload.Invoke();
                return true;
            }

            return false;
        }

        public override bool IsReloading()
        {
            return reloadTimer.CurrentTime > 0;
        }

        public override string GetAmmo()
        {
            if (maxAmmoCount == 0)
                return currentMagazineBulletCount.ToString();

            float magazineCount = totalAmmoCount / magazineAmmoCapacity;
            if (currentMagazineBulletCount == magazineAmmoCapacity)
                magazineCount--; // tick down after reload

            return currentMagazineBulletCount + " | " + magazineCount;
        }

        public override int GetAmmoLeftInMagazine()
        {
            return currentMagazineBulletCount;
        }

        public override void SpendAmmo(int ammoToSpend)
        {
            if (ammoToSpend > currentMagazineBulletCount)
                Debug.LogWarning("Spent more ammo than available. Values may appear negative.");

            currentMagazineBulletCount -= ammoToSpend;
            totalAmmoCount -= ammoToSpend;
        }        

        public override void AddAmmo(int ammoToAdd)
        {
            totalAmmoCount += ammoToAdd;
            currentMagazineBulletCount = Mathf.Clamp(currentMagazineBulletCount + ammoToAdd, 0, magazineAmmoCapacity);
        }
    }
}
