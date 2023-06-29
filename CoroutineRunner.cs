using System.Collections;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// This Monobehavior should be used to run coroutines that shouldn't stop if the original monobehavior is disabled/destroyed.
    /// </summary>
    public class CoroutineRunner : SingletonMonoBehavior<CoroutineRunner>
    {
        /// <summary>
        /// Calls a action after a given delay
        /// </summary>
        /// <param name="callback">The action to be called.</param>
        /// <param name="delay">The delay to be waited before callin the action, in seconds.</param>
        /// <param name="useUnscaledTime">Should the delay use unscaled time?</param>
        public static void CallAfterDelay(System.Action callback, float delay, bool useUnscaledTime = false)
        {
            if(callback == null)
                return;

            if(delay <= 0)
            {
                callback();
                return;
            }

            IEnumerator DelayedCoroutine()
            {
                if(useUnscaledTime)
                    yield return new WaitForSecondsRealtime(delay);
                else
                    yield return new WaitForSeconds(delay);

                callback?.Invoke();
            }

            RunCoroutine(DelayedCoroutine());

        }

        /// <summary>
        /// Start Coroutine attached to the CoroutineRunner Gameobject.
        /// </summary>
        /// <param name="routine">The routine to be started attached to the CoroutineRunner Gameobject.</param>
        public static Coroutine RunCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        /// <summary>
        /// Stop a given Coroutine attached to the CoroutineRunner, always use this method to StopCoroutines that were started by the CoroutineRunner
        /// </summary>
        /// <param name="coroutine">The coroutine to be stopped.</param>
        public static void StopMyCoroutine(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        /// <summary>
        /// Stop All Coroutines attached to the CoroutineRunner.
        /// </summary>
        public static void StopAllMyCoroutines()
        {
            Instance.StopAllCoroutines();
        }

        protected override void AdditionalOnDestroy()
        {
            StopAllCoroutines();
        }
    }
}