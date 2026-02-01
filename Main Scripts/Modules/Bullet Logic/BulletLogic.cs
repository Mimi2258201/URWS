using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    public abstract class BulletLogic : MonoBehaviour
    {
        [Tooltip("Where the bullet is spawned. It is recommended to use the camera/head for hitscan and the nozzle of the weapon for projectiles.")]
        public Transform bulletOriginTransform;        

        public virtual void PerformFire(Transform origin, Vector3 deviation = default(Vector3))
        {
            if (!bulletOriginTransform)
                throw new System.InvalidOperationException("Invalid origin transform given - cannot fire Ranged Weapon " + gameObject.name);            
        }

        protected Vector3 RotateForwardByEuler(Quaternion startRotation, Vector3 eulerRotation)
        {
            Quaternion localRotation = Quaternion.Euler(-1 * eulerRotation.y, eulerRotation.x, eulerRotation.z);
            Quaternion worldRotation = startRotation * localRotation;
            return worldRotation * Vector3.forward;
        }        

        private void Reset()
        {
            var modules = GetComponents<BulletLogic>();
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
