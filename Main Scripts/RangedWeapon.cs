using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Ranged Weapon", 0)]    
    public class RangedWeapon : MonoBehaviour
    {
        #region Modules
        [HideInInspector] public BulletLogic bulletLogic;
        [HideInInspector] public WeaponLogic weaponLogic;
        [HideInInspector] public Magazine magazine;     
        
        [HideInInspector] public RecoilExtension recoilExtension;       
        [HideInInspector] public MultiBarrelExtension multiBarrelExtension;
        [HideInInspector] public ShotgunBarrelExtension shotgunBarrelExtension;
        [HideInInspector] public EjectionExtension ejectionExtension;
        [HideInInspector] public WarmupExtension warmupExtension;
        [HideInInspector] public OverheatExtension overheatExtension;
        #endregion             

        [Tooltip("Called every time the weapon activates.For example, in a weapon with a burst fire, it will trigger for every bullet in the burst.")]
        public UnityEvent OnFire;  

        public Vector3 recoilRotation { get; private set; }

        private bool isTriggerDown;       

        //// Potential Aiming Down Sights (ADS) settings
        //public Camera weaponCamera; // assumes a URP camera stack / overlay camera
        //public ADSExtension ADSExtension;
        //public bool changeFOVOnAim = true;
        //public float regularWeaponFOV = 30f;
        //public float zoomedWeaponFOV = 10f;
        //public float inaccuracyMultiplierOnZoom = 0.4f;      


        private void Reset()
        {
            gameObject.AddComponent<HitscanBulletLogic>();
            gameObject.AddComponent<SingleFireLogic>();
            gameObject.AddComponent<BasicMagazine>();
        }        

        private void Awake()
        {
            bulletLogic = GetComponent<BulletLogic>();
            weaponLogic = GetComponent<WeaponLogic>();
            magazine = GetComponent<Magazine>();

            recoilExtension = GetComponent<RecoilExtension>();
            multiBarrelExtension = GetComponent<MultiBarrelExtension>();
            shotgunBarrelExtension = GetComponent<ShotgunBarrelExtension>();
            ejectionExtension = GetComponent<EjectionExtension>();
            warmupExtension = GetComponent<WarmupExtension>(); 
            warmupExtension?.OnWarmupComplete.AddListener(WarmupForceTrigger);
            overheatExtension = GetComponent<OverheatExtension>(); 
        }

        private void FixedUpdate()
        {
            // auto fire
            if (CanFire() && isTriggerDown && !weaponLogic.ReleaseTriggerOnFire())
                weaponLogic.Trigger(this);            
        }
        
        [Tooltip("'Pulling the trigger' of the weapon.")]
        public void OnTriggerDown()
        {
            isTriggerDown = true;

            if (CanFire())
            {
                if (warmupExtension)
                {
                    warmupExtension.StartWarmup();
                }
                else
                    weaponLogic.Trigger(this);

            }            
        }

        private void WarmupForceTrigger()
        {
            weaponLogic.Trigger(this);
        }

        [Tooltip("Tell the weapon that its trigger is no longer being held. Technically only necessary for auto/continuous weapons.")]
        public void OnTriggerUp()
        {
            if (isTriggerDown)
            {
                isTriggerDown = false;

                warmupExtension?.CancelWarmup();
            }
        }
        

        [Tooltip("Returns true only if it was a successful reload.")]
        public bool TryReload()
        {
            if (!weaponLogic.IsFiring())
            {
                return magazine.Reload();
            }

            return false;
        }


        public void HandleFire(int AmmoToSpend)
        {
            #region Weapon Properties Logic
            

            magazine.SpendAmmo(AmmoToSpend);              

            // Forcefully release trigger so that trigger needs to be pulled again to fire
            if (weaponLogic.ReleaseTriggerOnFire())
            {
                isTriggerDown = false;
            }

            ejectionExtension?.Eject();
            overheatExtension?.AddHeat();            
            #endregion

            #region Firing Logic
            List<Transform> fireTransforms = new List<Transform>();

            fireTransforms.Add(bulletLogic.bulletOriginTransform);            

            // Additional barrels
            if (multiBarrelExtension)
            {
                if (multiBarrelExtension.additionalBarrels.Length > 0)
                {
                    fireTransforms.AddRange(multiBarrelExtension.GetBarrels());                    
                }
            }

            // Add to recoil multiplier
            recoilExtension?.TickRecoil();

            // Fire bullets from every barrel
            foreach (var fireTransform in fireTransforms)
            {
                // Multi Fire
                if (shotgunBarrelExtension)
                {
                    for (int i = 0; i < shotgunBarrelExtension.bulletsPerFire; i++)
                    {
                        Vector3 deviation = AdditionalFunctions.GetRandomPointInCircle(shotgunBarrelExtension.maxSpreadAngle);
                        if (recoilExtension)
                            deviation += recoilExtension.PerformRecoil();

                        recoilRotation = deviation;
                        bulletLogic.PerformFire(fireTransform, deviation);

                        // Debug which only fires at the edges of the cone
                        //PerformFire(fireTransform, new Vector3(maxSpreadAngle * Mathf.Cos(angle), maxSpreadAngle * Mathf.Sin(angle), 0f));
                    }                    
                }
                else
                {
                    // Do raycast/spawn projectile
                    Vector3 deviation = Vector3.zero;
                    if (recoilExtension)
                    {
                        recoilRotation = recoilExtension.PerformRecoil();
                        if (recoilExtension.recoilType == RecoilType.AddInaccuracy)
                            deviation = recoilRotation;
                    }
                    
                    bulletLogic.PerformFire(fireTransform, deviation);
                }

            }

            #endregion

            // Call event
            OnFire.Invoke();
        }        

        public bool CanFire()
        {            
            // Check Timers
            if (magazine.IsReloading())
                return false;
            
            if (weaponLogic.IsFiring())
                return false;
            
            if (warmupExtension)
            {
                if (warmupExtension.isDoingWarmup)
                    return false;
            }
            
            // Check Ammo
            if (true) // Magazine_IsEnabled
            {
                int AmmoToBeConsumed = 1;

                // Check Multi barrel
                if (multiBarrelExtension)
                {
                    AmmoToBeConsumed += multiBarrelExtension.GetNumberOfAdditionalAmmoToConsume();
                }

                if (AmmoToBeConsumed > magazine.GetAmmoLeftInMagazine())
                    return false;                  
            }            
            
            if (overheatExtension)
            {
                if (overheatExtension.HasOverheated())
                    return false;
            }
            
            return true;
        }        
    }

    #region Enums
    public enum WarmupType
    {
        DelayedFire,
        HoldTriggerRequired,
    }
    #endregion

    #region Structs

    public struct Timer
    {
        public float CurrentTime;
        public float maxTime { get; private set; }

        public Timer(float MaxTime)
        {
            maxTime = MaxTime;
            CurrentTime = MaxTime;
        }
    }

    #endregion
}
