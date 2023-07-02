using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Misc
{
    public class DestroyParticleOnComplete : MonoBehaviour
    {
        public float waitAfterComplete = 0;

        private ParticleSystem part;

        private void Start()
        {
            part = GetComponent<ParticleSystem>();

            if(part)
            {
                Utils.Core.CoroutineRunner.RunCoroutine(WaitForParticleToEnd());
            }
            else
            {
                Destroy(this);
            }
        }

        private IEnumerator WaitForParticleToEnd()
        {
            yield return new WaitWhile(() => part.IsAlive());

            if(waitAfterComplete > 0)
                yield return new WaitForSeconds(waitAfterComplete);
        
            Destroy(part.gameObject);
        }
    }
}
