using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Extensions/Recoil/Vector Array Recoil", 1)]
    public class VectorArrayRecoil : RecoilExtension
    {
        [Tooltip("Use this to plot out exactly how the recoil should behave, values may be interpolated.")]
        public Vector2[] recoilVectorPath = new Vector2[0];

        [Tooltip("It is recommended to have a number of the last values loop so that there is no clear end to this finite list.\nThis variable indicates the first index in this loop, and will be the next position used after the last position is reached.")]
        public int recoilVectorPathLoopStartIndex = 0;



        public override Vector3 PerformRecoil()
        {
            Vector3 deviation = Vector3.zero;

            int index = (int)recoilMultiplier;
            int nextIndex = index + 1;
            float alpha = recoilMultiplier - index;

            if (nextIndex >= recoilVectorPath.Length)
            {
                nextIndex = recoilVectorPathLoopStartIndex;
                recoilMultiplier = nextIndex + alpha - 1f;
            }

            deviation = Vector2.Lerp(recoilVectorPath[index], recoilVectorPath[nextIndex], alpha);

            return deviation;
        }

        public override float GetMultiplierPercentage()
        {
            return Mathf.Clamp01(recoilMultiplier / recoilVectorPathLoopStartIndex);
        }

        public override void TickRecoil()
        {
            recoilMultiplier++;
        }
    }

   
   

}
