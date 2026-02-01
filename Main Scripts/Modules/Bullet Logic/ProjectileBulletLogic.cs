using UnityEngine;

namespace UniversalRangedWeaponSystem
{    
    [AddComponentMenu("Weapon System/Bullet Logic/Projectile Behaviour", 2)]
    public class ProjectileBulletLogic : BulletLogic
    {
        [Tooltip("The projectile gameobject prefab which will be instantiated on weapon fire.")]
        public Rigidbody projectilePrefab;

        [Tooltip("The axis on which the projectile will be fired relative to the forward of 'Bullet Origin Transform'.")]
        public Vector3 projectileForceDirection = Vector3.zero;

        [Tooltip("The magnitude of the force applied to the projectile when the weapon is fired.")]
        public float projectileForce = 50f;

        [Tooltip("The maximum number of projectiles created by this specific weapon in the scene before projectiles begin getting reused by the pooler.\nZero indicates there is no maximum, and the pooler will not be used.")]
        public int maxConcurrentProjectiles = 50;



        private Pool<Rigidbody> projectilePool = new Pool<Rigidbody>(null, 0);



        public override void PerformFire(Transform origin, Vector3 deviation = default(Vector3))
        {
            base.PerformFire(origin, deviation);

            if (projectilePrefab != null)
            {
                Rigidbody projectile;
                // Pooling
                if (maxConcurrentProjectiles > 0) // Use pool
                {
                    projectile = projectilePool.SpawnObject(origin.position, origin.rotation);
#if UNITY_6000_0_OR_NEWER
                    projectile.linearVelocity = Vector3.zero;
#else
                    projectile.velocity = Vector3.zero;
#endif
                    projectile.angularVelocity = Vector3.zero;
                }

                else
                    projectile = Instantiate<Rigidbody>(projectilePrefab, origin.position, origin.rotation);

                
                Vector3 force = RotateForwardByEuler(origin.rotation, projectileForceDirection + deviation);
                force *= projectileForce;
                projectile.AddForce(force, ForceMode.Impulse);
                
            }
            else
                Debug.LogAssertion("Could not fire Ranged Weapon as no projectile prefab was detected on " + gameObject.name);
        }

        private void FixedUpdate()
        {
            if (projectilePool.maxCount != maxConcurrentProjectiles || projectilePool.prefab != projectilePrefab)
            {                
                projectilePool = new Pool<Rigidbody>(projectilePrefab, maxConcurrentProjectiles);
            }
        }        

        private void OnDestroy()
        {
            if (projectilePool != null)
                projectilePool.Dispose();
        }
    }
}
