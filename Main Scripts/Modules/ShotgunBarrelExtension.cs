using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Extensions/Shotgun Extension", 2)]
    public class ShotgunBarrelExtension : MonoBehaviour
    {
        [Tooltip("The number of projectiles/raycasts that are sent out each fire of the weapon.")]
        public int bulletsPerFire = 10;

        [Range(0f, 180f), Tooltip("The angle (in degrees) between the centre fire line and the edge of the cone of fire.")]
        public float maxSpreadAngle = 15f;
    }
}
