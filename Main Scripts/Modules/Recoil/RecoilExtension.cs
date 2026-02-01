using UnityEngine;

namespace UniversalRangedWeaponSystem
{    
    public abstract class RecoilExtension : MonoBehaviour
    {
        [Tooltip("The speed (degrees per second) at which the recoil effect is gradually undone and returns to normal.")]
        public float recoilCooldownSpeed = 10f;

        [Tooltip("Whether or not the calculated recoil should affect the trajectory of the bullets.\nCalculated recoil can be accessed from the weapon regardless.")]
        public RecoilType recoilType = RecoilType.AddInaccuracy;

        public float recoilMultiplier { get; protected set; }        


        public virtual Vector3 PerformRecoil() { return Vector3.zero; }
        public virtual void TickRecoil() { }
        public virtual float GetMultiplierPercentage() { return 0f; }

        private void FixedUpdate()
        {
            recoilMultiplier -= recoilCooldownSpeed * Time.deltaTime;
            recoilMultiplier = Mathf.Clamp(recoilMultiplier, 0, float.MaxValue);
        }

        private void Reset()
        {
            var modules = GetComponents<RecoilExtension>();
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

    public enum RecoilType
    {
        AddInaccuracy,
        ReturnOnly
    }
}
