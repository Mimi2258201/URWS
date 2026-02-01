using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UniversalRangedWeaponSystem.Examples
{
    [AddComponentMenu("Weapon System/Examples/Projectile", 1)]
    [RequireComponent(typeof(Rigidbody))] // to ensure OnCollisionEnter fires
    public class Projectile : MonoBehaviour
    {
        [Tooltip("The time in seconds that this object will exist in the scene for before disabling itself.")]
        public float aliveTime = 5f;

        [Tooltip("A Layer Mask of all layers that should destroy this object on collision.\nLeave this blank if you don't want that behaviour.")]
        public LayerMask collisionLayerMask;

        public UnityEvent<Collision> OnHit;
        
        private void OnEnable()
        {
            // Stop coroutines first so it properly resets when recycled by the object pooler
            StopAllCoroutines();
            StartCoroutine(DestroyOnTimer(aliveTime));        
        }

        // "kill" on timer end or on collision hit
        private IEnumerator DestroyOnTimer(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collisionLayerMask == (collisionLayerMask | (1 << collision.gameObject.layer)))
            {
                OnHit.Invoke(collision);
                gameObject.SetActive(false);
            }
        }
    }

}
