using UnityEngine;

namespace UniversalRangedWeaponSystem.Examples
{
    [AddComponentMenu("Weapon System/Examples/Weapon Pad", 1)]
    public class WeaponPad : MonoBehaviour
    {
        [Tooltip("The prefab object that is to be assigned to the player while standing on the Weapon Pad.")]
        public RangedWeapon weaponPrefab;
        private WeaponSocket playerSocket;        

        private void Awake()
        {
            playerSocket = FindFirstObjectByType<WeaponSocket>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerSocket.AssignWeapon(weaponPrefab);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerSocket.AssignWeapon(null);
            }
        }
    }
}
