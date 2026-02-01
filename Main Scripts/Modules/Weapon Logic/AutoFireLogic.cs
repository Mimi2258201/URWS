using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Weapon Behaviour/Auto Fire", 3)]
    public class AutoFireLogic : WeaponLogic
    {
        public override bool ReleaseTriggerOnFire()
        {
            return false;
        }
    }
}
