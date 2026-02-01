using UnityEngine;
using UnityEngine.Events;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Extensions/Overheat Extension", 1)]
    public class OverheatExtension : MonoBehaviour
    {
        [Tooltip("The amount of heat added to the weapon every time it fires. Its overheat limit is arbitrarily set to 10.")]
        public float heatPerFire = 1f;

        [Tooltip("The time (in seconds) to begin the cooldown.")]
        public float cooldownDelay = 0.5f;

        [Tooltip("Cooldown refers to the weapon's heat returning to normal after not firing. The time (in seconds) for the weapon's heat to return to 0.\nThis value will be used relative to how much heat it currently has.")]
        public float cooldownTime = 1f;

        [Tooltip("The time (in seconds) that the weapon should be disabled for once the weapon has overheated.")]
        public float disableTime = 1f;

        [Tooltip("Called when weapon reaches the heat limit and gets disabled.")]
        public UnityEvent OnOverheat;
        

        public float currentHeat { get; private set; }
        public float overheatLimit { get; private set; } = 10f;


        private Timer cooldownDelayTimer;
        private Timer overheatDisableTimer;


        private void FixedUpdate()
        {
            if (cooldownDelayTimer.CurrentTime > 0)
                cooldownDelayTimer.CurrentTime = Mathf.Clamp(cooldownDelayTimer.CurrentTime - Time.deltaTime, 0, float.MaxValue);

            if (currentHeat > 0 && cooldownDelayTimer.CurrentTime == 0)
                currentHeat = Mathf.Clamp(currentHeat - (Time.deltaTime * cooldownTime * overheatLimit), 0, float.MaxValue);

            if (overheatDisableTimer.CurrentTime > 0)
                overheatDisableTimer.CurrentTime = Mathf.Clamp(overheatDisableTimer.CurrentTime - Time.deltaTime, 0, float.MaxValue);            

            if (cooldownDelayTimer.maxTime != cooldownTime)
                cooldownDelayTimer = new Timer(cooldownTime);

            if (overheatDisableTimer.maxTime != disableTime)
                overheatDisableTimer = new Timer(disableTime);
        }

        public void AddHeat()
        {            
            currentHeat += heatPerFire;
            cooldownDelayTimer.CurrentTime = cooldownDelayTimer.maxTime;

            // Has overheated?
            if (currentHeat > overheatLimit)
            {
                overheatDisableTimer.CurrentTime = overheatDisableTimer.maxTime;
                currentHeat = 0;
                OnOverheat.Invoke();
            }
            
        }

        public bool HasOverheated()
        {
            return overheatDisableTimer.CurrentTime > 0;
        }
    }
}
