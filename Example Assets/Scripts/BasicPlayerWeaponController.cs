using UnityEngine;

namespace UniversalRangedWeaponSystem.Examples
{
    [AddComponentMenu("Weapon System/Examples/Basic Player Weapon Controller", 1)]
    public class BasicPlayerWeaponController : MonoBehaviour
    {
        [SerializeField, Tooltip("The Ranged Weapon component that this controller should control.")]
        private RangedWeapon rangedWeapon;

        private void Start()
        {
            WeaponSocket.OnWeaponAttached += (RangedWeapon newRangedWeapon) => rangedWeapon = newRangedWeapon;            
        }

        private void Update()
        {
            if (rangedWeapon == null) return;

            // Input down
            if (InputManager.instance.wasPrimaryFireActionPressedThisFrame)
            {
                rangedWeapon.OnTriggerDown();
            }

            // Input up
            if (InputManager.instance.wasPrimaryFireActionReleasedThisFrame)
            {
                rangedWeapon.OnTriggerUp();
            }

            // Reload trigger
            if (InputManager.instance.wasSecondaryFireActionPressedThisFrame)
            {
                rangedWeapon.TryReload();
            }
        }
    }
}
