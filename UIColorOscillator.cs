using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{   
    /// <summary>
    /// This component oscillates the color of an attached graphic component between it's initial color and the color stored in the TargetColor variable
    /// </summary>
    public class UIColorOscillator : MonoBehaviour
    {   
        /// <summary>
        /// Enables or Disables the Oscillating effect of this Component
        /// </summary>
        public bool ShouldOscillate = true;

        /// <summary>
        /// The TargetColor that this Component should oscillate to
        /// </summary>
        public Color TargetColor = Color.white;

        /// <summary>
        /// Interval of oscillation
        /// </summary>
        public float Interval = 1;

        bool started = false;
        Graphic m_graphic = null;
        private Color startColor;

        /// <summary>
        /// Return the current color of the attached graphic to it's initial color
        /// </summary>
        public void ResetColor()
        {
            if(m_graphic)
                m_graphic.color = startColor;
        }

        /// <summary>
        /// Sets a new initial color
        /// </summary>
        public void SetStartColor(Color startColor)
        {
            this.startColor = startColor;
        }

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            if(!started)
            {
                started = true;
                m_graphic = GetComponent<Graphic>();

                if(m_graphic != null)
                {
                    startColor = m_graphic.color;

                    CoroutineRunner.RunCoroutine(Run());
                }
                else
                {
                    Debug.LogError("Failed to find target to apply color!");
                }
            }
        }

        private IEnumerator Run()
        {
            while(true)
            {
                // Return color back to the initial color
                ResetColor();
                
                // Wait until ShouldOscillate is true and Interval is a positive value
                if(!ShouldOscillate || Interval <= 0)
                    yield return new WaitUntil(() => ShouldOscillate && Interval > 0);

                float halfInterval = Interval/2;
                float accumulatedTime = 0;

                // Lerp Graphic's color from the startColor to the targetColor in halfInterval seconds
                while(accumulatedTime < halfInterval)
                {
                    accumulatedTime += Time.deltaTime;

                    if(m_graphic)
                        m_graphic.color = Color.Lerp(startColor, TargetColor, accumulatedTime/halfInterval);

                    yield return null;
                }

                accumulatedTime = 0;

                // Lerp Graphic's color from the targetColor to the startColor in halfInterval seconds
                while(accumulatedTime < halfInterval)
                {
                    accumulatedTime += Time.deltaTime;

                    if(m_graphic)
                        m_graphic.color = Color.Lerp(TargetColor, startColor, accumulatedTime/halfInterval);

                    yield return null;
                }
            }   
        }
    }
}