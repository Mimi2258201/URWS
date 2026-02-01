using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Extensions/Ejection Extension", 1)]
    public class EjectionExtension : MonoBehaviour
    {
        [Tooltip("The prefab that will be instanced at Shell Exit Point.")]
        public Rigidbody shellPrefab;

        [Tooltip("New prefabs will be spawned here with this object's world rotation.")]
        public Transform shellExitPoint;

        [Tooltip("The magnitude of the impulse force applied to shell.")]
        public float ejectionForce = 50f;

        [Tooltip("The maximum number of ejected shells created by this specific weapon in the scene before shells begin getting reused by the pooler.\nZero indicates there is no maximum, and the pooler will not be used.")]
        public int maxConcurrentEjectedShells = 5;


        public Pool<Rigidbody> ejectionPool;

        private void Awake()
        {
            if (shellPrefab)
                ejectionPool = new Pool<Rigidbody>(shellPrefab, maxConcurrentEjectedShells);
        }

        private void FixedUpdate()
        {
            
            if (ejectionPool.maxCount != maxConcurrentEjectedShells || ejectionPool.prefab != shellPrefab)
            {
                ejectionPool = new Pool<Rigidbody>(shellPrefab, maxConcurrentEjectedShells);
            }
            
        }

        public void Eject()
        {
            

            // Ejection
            if (shellPrefab && shellExitPoint)
            {
                Rigidbody shell;
                // Pooling
                if (maxConcurrentEjectedShells > 0)
                {
                    shell = ejectionPool.SpawnObject(shellExitPoint.position, shellExitPoint.rotation);
#if UNITY_6000_0_OR_NEWER
                    shell.linearVelocity = Vector3.zero;
#else
                    shell.velocity = Vector3.zero;
#endif
                    shell.angularVelocity = Vector3.zero;
                }

                // Regular instantiating
                else
                    shell = Instantiate(shellPrefab, shellExitPoint.position, shellExitPoint.rotation);

                shell.AddForce(shellExitPoint.forward * ejectionForce, ForceMode.Impulse);
            }
            else
            {
                string errorSource = !shellPrefab ? "shell prefab" : "Shell Exit Point (Transform)";
                Debug.LogAssertion($"Could not eject shell as no {errorSource} was detected on {gameObject.name}");
            }

        }

        private void OnDestroy()
        {
            if (ejectionPool != null)
                ejectionPool.Dispose();
        }
    }
}
