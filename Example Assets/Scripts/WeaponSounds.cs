using UnityEngine;

namespace UniversalRangedWeaponSystem.Examples
{
    [RequireComponent(typeof(RangedWeapon)), AddComponentMenu("Weapon System/Examples/Weapon Sound Effect", 1)]
    public class WeaponSounds : MonoBehaviour
    {
        [Tooltip("The sound clip to be played every time the gun fires.")]
        public AudioClip soundEffect;
        private AudioSource audioSource;
        
        void Start()
        {
            RangedWeapon rangedWeapon = GetComponent<RangedWeapon>();
            rangedWeapon.OnFire.AddListener(OnFire);            

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = soundEffect;
            audioSource.volume = 0.3f;
        }

        private void OnFire()
        {
            if (soundEffect)
            {
                float pitch = Random.Range(0.9f, 1.1f);
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(soundEffect);  
            }
            else
                Debug.LogAssertion("Could not play sound on Ranged Weapon as audio clip was select for " + gameObject.name);
        }     
    }
}