using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Extensions/Recoil/Random Direction Recoil", 1)]
    public class RandomDirectionRecoil : RecoilExtension
    {
        [Tooltip("In which general direction the recoil should be applied.")]
        public RecoilDirections recoilDirections = RecoilDirections.UpAndSides;

        [Tooltip("An axis separated multiplier for the recoil (values between 0-1 recommended).\nSetting values to 1 indicates regular recoil intensity, lower values mean less intensive recoil.")]
        public Vector2 axisBias = new Vector2(0.5f, 1f);

        [Tooltip("Equivalent to the angle (in degrees) added to the recoil per shot.")]
        public float perFireMultiplier = 2f;

        [Tooltip("The angle (in degrees) between the starting direction and the furthest possible vector from this point.")]
        public float maxInaccuracyAngle = 20f;


        public override Vector3 PerformRecoil()
        {
            Vector3 deviation = Vector3.zero;

            deviation = AdditionalFunctions.GetRandomPointInCircle(recoilMultiplier);
            deviation = new Vector2(deviation.x * axisBias.x, deviation.y * axisBias.y);

            switch (recoilDirections)
            {
                case RecoilDirections.UpOnly:
                    deviation.x = 0f;
                    deviation.y = Mathf.Abs(deviation.y);
                    break;

                case RecoilDirections.SidesOnly:
                    deviation.y = 0f;
                    break;

                case RecoilDirections.UpAndSides:
                    // No Negative Y values
                    deviation.y = Mathf.Abs(deviation.y);
                    break;

                case RecoilDirections.OmniDirectional:
                    break;
            }

            return deviation;
        }

        public override void TickRecoil()
        {
            recoilMultiplier = Mathf.Clamp(recoilMultiplier + perFireMultiplier, 0, maxInaccuracyAngle);
        }

        public override float GetMultiplierPercentage()
        {
            maxInaccuracyAngle = Mathf.Clamp(maxInaccuracyAngle, 0.01f, float.MaxValue);
            return Mathf.Clamp01(recoilMultiplier / maxInaccuracyAngle);
        }
    }

    

    public enum RecoilDirections
    {
        UpOnly,
        UpAndSides,
        SidesOnly,
        OmniDirectional
    }

}
