using UnityEngine;
using UnityEngine.Events;

namespace UniversalRangedWeaponSystem
{
    [AddComponentMenu("Weapon System/Extensions/Warmup Extension", 1)]
    public class WarmupExtension : MonoBehaviour
    {
        [Tooltip("The time (in seconds) for the weapon to finish the warmup sequence and begin firing.")]
        public float timeDuration = 0.8f;

        [Tooltip("If the trigger is let go before the warmup has completed, should the weapon cancel the warmup?")]
        public bool isCancellable;

        [Tooltip("Called when triggering the warmup sequence.")]
        public UnityEvent OnWarmupStart;

        [Tooltip("Called when the warmup sequence ends early by untriggering the weapon.\nThis will only fire if 'Is Cancellable' is enabled.")]
        public UnityEvent OnWarmupCancel;

        [Tooltip("Called only once the warmup sequence successfully finishes.")]
        public UnityEvent OnWarmupComplete;

        [Tooltip("Called when the warmup ends, regardless of if it was completed or cancelled.")]
        public UnityEvent OnWarmupEnd;

        public bool isDoingWarmup { get; private set; }

        private Timer warmupTimer;

        public void StartWarmup()
        {
            isDoingWarmup = true;
            warmupTimer.CurrentTime = warmupTimer.maxTime;
            OnWarmupStart.Invoke();
        }

        public void CancelWarmup()
        {
            if (isCancellable)
            {
                isDoingWarmup = false;
                OnWarmupCancel?.Invoke();
                OnWarmupEnd?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            // tick
            if (warmupTimer.CurrentTime > 0)
                warmupTimer.CurrentTime = Mathf.Clamp(warmupTimer.CurrentTime - Time.deltaTime, 0, float.MaxValue);

            // update
            if (warmupTimer.maxTime != timeDuration)
                warmupTimer = new Timer(timeDuration);

            // Disable warmup
            if (warmupTimer.CurrentTime == 0f && isDoingWarmup)
            {                
                isDoingWarmup = false;
                OnWarmupComplete?.Invoke();                
                OnWarmupEnd?.Invoke();                
            }
        }
    }
}
