using System.Linq;
using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Bullet Logic/Penetrating Hitscan Behaviour", 1)]
    public class PenetratingHitscanBulletLogic : HitscanBulletLogic
    {
        [Tooltip("Layers that the raycast will consider penetrable, ignores any values that aren't on the hitscan layermask.")]
        public LayerMask penetrationLayerMask = 1;

        [Tooltip("The number of objects a bullet can pass through before stopping. Zero indicates that there is no limit, and it can pass through these layers indefinitely.")]
        public int penetrationMaxCount = 5;                   

        public override void PerformFire(Transform origin, Vector3 deviation = default(Vector3))
        {
            base.PerformFire(origin, deviation);

            // Perform Penetrative ray
            RaycastHit[] hits;
            hits = Physics.RaycastAll(origin.position, RotateForwardByEuler(origin.rotation, deviation), Mathf.Infinity, hitscanLayerMask);

            // Loop through all hits of the penetrative ray
            hits.OrderBy(hit => Vector3.Distance(origin.position, hit.point));
            int hitCount = 0;

            for (int i = 0; i < hits.Length; i++)
            {
                bool stopLoop = false;
                // If it hits unpenetrable layer then stop
                if ((hitscanLayerMask & (1 << hits[i].collider.gameObject.layer)) == 0)
                {
                    stopLoop = true;
                }

                SpawnDecal(hits[i]);
                hitCount++;
                OnRaycastHit.Invoke(hits[i]);

                if (hitCount == penetrationMaxCount + 1 && penetrationMaxCount > 0)
                    break;

                if (stopLoop)
                    break;
            }
                                   
        }        
    }
}
