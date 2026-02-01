using System.Collections.Generic;
using UnityEngine;

namespace UniversalRangedWeaponSystem.Examples
{
    [AddComponentMenu("Weapon System/Examples/Weapon Socket", 1)]
    public class WeaponSocket : MonoBehaviour
    {
        [Tooltip("Assign this if you want to override all weapons to a specific Bullet Origin such as the player camera.")]
        public Transform bulletOriginTransformOverride;        

        private RangedWeapon rangedWeapon;

        public delegate void WeaponAttachedDelegate(RangedWeapon attachedWeapon);
        public static event WeaponAttachedDelegate OnWeaponAttached;

        
        public void AssignWeapon(RangedWeapon newRangedWeapon)
        {
            // Remove old weapon
            if (rangedWeapon != null || newRangedWeapon == null)
                Destroy(rangedWeapon.gameObject);
            if (newRangedWeapon == null) return;


            // Apply settings to new weapon
            rangedWeapon = Instantiate<RangedWeapon>(newRangedWeapon, transform, transform);
            rangedWeapon.transform.localRotation = Quaternion.identity;
            rangedWeapon.transform.localPosition = Vector3.zero;
            
            // Assign additional barrels if main barrel is not located on the prefab.
            if (rangedWeapon.multiBarrelExtension && rangedWeapon.bulletLogic.bulletOriginTransform == null)
            {
                AssignAdditionalBarrels();
            }

            // Assign main barrel location if needed
            if (rangedWeapon.bulletLogic.bulletOriginTransform == null)
                rangedWeapon.bulletLogic.bulletOriginTransform = bulletOriginTransformOverride;

            OnWeaponAttached?.Invoke(rangedWeapon);            
        }

        private void AssignAdditionalBarrels()
        {
            List<GameObject> barrels = new List<GameObject>();

            foreach (Transform child in bulletOriginTransformOverride)
            {
                if (child.CompareTag("Additional Barrel"))
                {
                    barrels.Add(child.gameObject);
                }
            }

            for (int i = 0; i < rangedWeapon.multiBarrelExtension.additionalBarrels.Length; i++)
            {
                if (barrels.Count > 0)
                    rangedWeapon.multiBarrelExtension.additionalBarrels[i] = barrels[0];
                else
                    rangedWeapon.multiBarrelExtension.additionalBarrels[i] = null;
            }
        }
    }
}
