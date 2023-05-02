using System.Collections;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// This Monobehavior should be used to run coroutines that shouldn't stop if the original monobehavior is disabled/destroyed.
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        // Create a singleton reference that instantiate itself whenever it's referenced for the first time in a scene.
        private static CoroutineRunner Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = FindObjectOfType<CoroutineRunner>();

                    if(m_instance == null)
                    {
                        m_instance = (new GameObject("Coroutine Runner")).AddComponent<CoroutineRunner>();
                    }

                    m_instance.Start();
                }

                return m_instance;
            }
        }

        private static CoroutineRunner m_instance = null;

        private bool hasStarted = false;

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

        private void Start()
        {
            if(!hasStarted)
            {
                hasStarted = true;

                if(m_instance == null)
                {
                    m_instance = this;
                }
                else
                {
                    if(m_instance != this)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            if(m_instance == this)
                m_instance = null;
        }
    }
}