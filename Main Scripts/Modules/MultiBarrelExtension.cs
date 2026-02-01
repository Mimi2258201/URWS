using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UniversalRangedWeaponSystem
{    
    [AddComponentMenu("Weapon System/Extensions/Multi-Barrel Extension", 1)]
    public class MultiBarrelExtension : MonoBehaviour
    {
        [Tooltip("These act as additional 'Bullet Origin Transform' for any extra barrels added.")]
        public GameObject[] additionalBarrels;

        [Tooltip("When enabled the barrels will be fired in order (starting with Bullet Origin Transform). When turned off they will all fire at once, every single fire.")]
        public bool cycleBetweenBarrels = false;

        [Tooltip("Whether a fire should consume the full proper amount of ammo. When enabled, each barrel of the weapon will consume one bullet, if disabled then only one bullet is used per fire.")]
        public bool consumeFullAmmo = false;

        public int barrelIndex { get; private set; } = 0; 
        

        public int GetNumberOfAdditionalAmmoToConsume()
        {
            if (consumeFullAmmo)
                return additionalBarrels.Length;
            else
                return 0;
        }

        public List<Transform> GetBarrels()
        {
            // Cycle barrels
            if (cycleBetweenBarrels)
            {
                List<Transform> barrels = new List<Transform>(); // Get next barrel transform       
                Transform origin = additionalBarrels[barrelIndex].transform;
                barrels.Add(origin);

                barrelIndex++; // Cycle index
                if (barrelIndex >= additionalBarrels.Length)
                    barrelIndex = 0;

                return barrels;
            }

            // All at once
            else            
                return additionalBarrels.Select(Barrel => Barrel.transform).ToList();
            
        }
    }
}
