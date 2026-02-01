using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UniversalRangedWeaponSystem.Examples
{
    [AddComponentMenu("Weapon System/Examples/Weapon Hud", 1)]
    public class WeaponHUD : MonoBehaviour
    {
        [SerializeField, Tooltip("The ranged weapon that the HUD should read from.")]
        private RangedWeapon rangedWeapon;
        private BasicMagazine magazine;
     
        public Image reloadUI;
        public Image overheatUI;
        public Image recoilUI;
        public Image fireUI;
        public Image warmupUI;
        [Tooltip("The text component that displays relevant info about how much ammo is in the weapon.")]
        public TextMeshProUGUI ammoText;

        private bool isSetup = false;

        private Timer reloadTimer;
        private bool doReload;

        private Timer fireCooldownTimer;
        private bool doFireCooldown;

        private Timer warmupTimer;
        private bool doWarmup;

        private Timer overheatTimer;
        private bool doDisable;

        private void Start()
        {
            WeaponSocket.OnWeaponAttached += (RangedWeapon newRangedWeapon) =>
            {                   
                OnAssign(newRangedWeapon);
            };
        }

        
        private void OnAssign(RangedWeapon assignedWeapon)
        {
            rangedWeapon = assignedWeapon;

            // UI
            if (ammoText)
                ammoText.enabled = true;
            if (fireUI)
                fireUI.enabled = true;
            if (recoilUI)
                recoilUI.enabled = rangedWeapon.recoilExtension;
            if (overheatUI)
                overheatUI.enabled = rangedWeapon.overheatExtension;            
            if (warmupUI)
                warmupUI.enabled = rangedWeapon.warmupExtension;
            if (reloadUI)
                reloadUI.enabled = magazine;
            magazine = rangedWeapon.magazine as BasicMagazine;

            // Events
            if (reloadUI)
                magazine.OnReload.AddListener(OnReload);
            if (fireUI)
                rangedWeapon.OnFire.AddListener(OnFire);
            if (overheatUI)
            rangedWeapon.overheatExtension?.OnOverheat.AddListener(OnOverheat);
            if (warmupUI)
            {
                rangedWeapon.warmupExtension?.OnWarmupStart.AddListener(OnWarmupStart);
                rangedWeapon.warmupExtension?.OnWarmupEnd.AddListener(OnWarmupEnd);
            }

            AdditionalFunctions.DelayUntilNextTick(UpdateAmmoText);            
            isSetup = true;
        }

        private void FixedUpdate()
        {
            // Safely function if no weapon is set
            if (rangedWeapon == null)
            {
                if (isSetup)
                {
                    // UI
                    reloadUI.enabled = false;
                    overheatUI.enabled = false;
                    recoilUI.enabled = false;
                    fireUI.enabled = false;
                    warmupUI.enabled = false;
                    ammoText.enabled = false;
                    isSetup = false;
                }
                return;
            }            

            if (doReload)
            {
                // Check timer start
                if (!reloadUI.enabled)
                    reloadUI.enabled = true;

                // Do timer things
                reloadTimer.CurrentTime -= Time.deltaTime;
                float alpha = reloadTimer.CurrentTime / reloadTimer.maxTime;
                reloadUI.transform.localScale = Vector3.one * Mathf.Clamp01(1 - alpha);

                // Check timer end
                if (reloadTimer.CurrentTime <= 0 )
                {
                    doReload = false;
                    UpdateAmmoText();
                }        
            }
            else if (reloadUI.enabled)
                reloadUI.enabled = false;

            if (doFireCooldown)
            {
                // Do timer things
                fireCooldownTimer.CurrentTime -= Time.deltaTime;
                float alpha = fireCooldownTimer.CurrentTime / fireCooldownTimer.maxTime;
                fireUI.fillAmount = 1 - alpha;

                // Check timer end
                if (fireCooldownTimer.CurrentTime <= 0)
                    doFireCooldown = false;
            }

            if (doWarmup)
            {
                // Do timer things
                warmupTimer.CurrentTime -= Time.deltaTime;
                float alpha = warmupTimer.CurrentTime / warmupTimer.maxTime;
                warmupUI.transform.localScale = Vector3.one * Mathf.Clamp01(1 - alpha);

                // Check timer end
                if (warmupTimer.CurrentTime <= 0)
                    doWarmup = false;
            }
            else if (warmupUI.enabled)
                warmupUI.enabled = false;

            if (rangedWeapon.overheatExtension && !doDisable)
            {
                float alpha = rangedWeapon.overheatExtension.currentHeat / rangedWeapon.overheatExtension.overheatLimit;
                overheatUI.color = new Color(1, 0, 0, Mathf.Lerp(0f, 0.9f, alpha));
            }
            else if (overheatUI.enabled)
                overheatUI.enabled = false;

            // Overheat disable
            if (doDisable)
            {
                overheatTimer.CurrentTime = Mathf.Clamp(overheatTimer.CurrentTime - Time.deltaTime, 0, float.MaxValue);

                // Randomly scale
                overheatUI.enabled = true;
                overheatUI.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, Mathf.Sin((overheatTimer.CurrentTime * 30f)+ Time.realtimeSinceStartup));
            

                // Reset
                if (overheatTimer.CurrentTime == 0)
                {
                    doDisable = false;
                    overheatUI.transform.localScale = Vector3.one;
                }
            }
        
            // Recoil                              
            if (rangedWeapon.recoilExtension)
                recoilUI.transform.localScale = Vector3.one * (1 + rangedWeapon.recoilExtension.GetMultiplierPercentage());            
        }

        private void OnReload()
        {
            if (rangedWeapon != null)
            {
                doReload = true;
                reloadTimer = new Timer(magazine.reloadTimeDuration);
            }
        }

        private void OnOverheat()
        {
            if (rangedWeapon != null)
            {
                doDisable = true;
                overheatTimer = new Timer(rangedWeapon.overheatExtension.disableTime);
                overheatUI.color = new Color(1, 0, 0, 0.9f);
            }
        }

        private void OnFire()
        {
            if (rangedWeapon != null)
            {
                doFireCooldown = true;
                fireCooldownTimer = new Timer(rangedWeapon.weaponLogic.minimumTimeBetweenShots);
            }

            UpdateAmmoText();
        }

        private void OnWarmupStart()
        {
            if (rangedWeapon != null)
            {
                doWarmup = true;
                warmupTimer = new Timer(rangedWeapon.warmupExtension.timeDuration);
                warmupUI.transform.localScale = Vector3.zero;
                warmupUI.enabled = true;            
            }
        }

        private void OnWarmupEnd()
        {
            doWarmup = false;
            warmupUI.enabled = false;
        }

        private void UpdateAmmoText()
        {
            if (rangedWeapon != null && ammoText != null)
            {
                ammoText.text = rangedWeapon.magazine.GetAmmo();                
            }
        }
    }
}
