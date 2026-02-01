using System;
using System.Collections;
using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    public static class AdditionalFunctions
    {
        private class CoroutineHelper : MonoBehaviour { }
        private static CoroutineHelper _instance;
        private static CoroutineHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("CoroutineHelper");
                    UnityEngine.Object.DontDestroyOnLoad(obj); // Ensure it persists between scenes
                    _instance = obj.AddComponent<CoroutineHelper>();
                }
                return _instance;
            }
        }

        public static Vector2 GetRandomPointInCircle(float radius)
        {
            float randAngle = UnityEngine.Random.value * 2 * Mathf.PI;
            float randRadius = radius * Mathf.Sqrt(UnityEngine.Random.value);
            return new Vector2(randRadius * Mathf.Cos(randAngle), radius * Mathf.Sin(randAngle));
        }

        public static void DelayUntilNextTick(Action action)
        {
            Run(_DelayUntilNextTick(action));
        }

        public static void DelayForSeconds(Action action, float secondsUnscaled)
        {
            Run(_DelayForSeconds(action, secondsUnscaled));
        }



        // Private helpers
        private static void Run(IEnumerator coroutine)
        {
            Instance.StartCoroutine(coroutine);
        }

        private static IEnumerator _DelayUntilNextTick(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        private static IEnumerator _DelayForSeconds(Action action, float secondsUnscaled)
        {
            yield return new WaitForSecondsRealtime(secondsUnscaled);
            action?.Invoke();
        }
    }
}
