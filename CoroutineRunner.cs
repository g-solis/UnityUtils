using System.Collections;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// This Monobehavior should be used to run coroutines that shouldn't stop if the original monobehavior is disabled/destroyed
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        // Create a singleton reference that instantiate itself whenever it's referenced for the first time in a scene
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
        /// Start Coroutine attached to the CoroutineRunner
        /// </summary>
        public static Coroutine RunCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        /// <summary>
        /// Stop All Coroutines attached to the CoroutineRunner
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