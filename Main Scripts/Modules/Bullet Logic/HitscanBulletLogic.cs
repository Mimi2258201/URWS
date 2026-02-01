using UnityEngine;
using UnityEngine.Events;


namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Bullet Logic/Hitscan Behaviour", 1)]
    public class HitscanBulletLogic : BulletLogic
    {

        [Tooltip("Select the layers that the raycast should consider a 'hit' all other layers are ignored the raycast will pass through them.")]
        public LayerMask hitscanLayerMask = 1;

        [Tooltip("The object that will be spawned where the raycast hits. If there are multiple raycasts, multiple prefabs will be spawned.")]
        public GameObject decalPrefab;

        [Tooltip("The maximum number of hitscan decals created by this specific weapon in the scene before decals begin getting reused by the pooler.\nZero indicates there is no maximum, and the pooler will not be used.")]
        public int maxConcurrentHitDecals = 5;

        [Tooltip("Called when the raycast hits any object with a collider. Returns the produced RaycastHit.")]
        public UnityEvent<RaycastHit> OnRaycastHit;


        private Pool<Transform> decalPool = new Pool<Transform>(null, 0);        


        public override void PerformFire(Transform origin, Vector3 deviation = default(Vector3))
        {
            base.PerformFire(origin, deviation);

            // regular raycast            
            RaycastHit hit;
            if (Physics.Raycast(origin.position, RotateForwardByEuler(origin.rotation, deviation), out hit, Mathf.Infinity, hitscanLayerMask))
            {
                SpawnDecal(hit);
                OnRaycastHit.Invoke(hit);
            }                                   
        }

        private void FixedUpdate()
        {
            if (decalPrefab)
            {
                if (decalPrefab.transform != decalPool.prefab || maxConcurrentHitDecals != decalPool.maxCount)
                {
                    decalPool = new Pool<Transform>(decalPrefab.transform, maxConcurrentHitDecals);
                }
            }
        }        

        protected void SpawnDecal(RaycastHit hit)
        {
            if (hit.collider == null)
                return;

            if (decalPrefab)
            {
                if (maxConcurrentHitDecals > 0)
                {
                    decalPool.SpawnObject(hit.point, Quaternion.LookRotation(hit.normal));                    
                }
                else
                    Instantiate(decalPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        private void OnDestroy()
        {
            if (decalPool != null)
                decalPool.Dispose();
        }
    }
}
