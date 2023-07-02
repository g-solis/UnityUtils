using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils.Core;

namespace Utils.Animation
{   
    /// <summary>
    /// This component oscillates the color of an attached graphic component between it's initial color and the color stored in the TargetColor variable.
    /// </summary>
    public class ColorOscillator : MonoBehaviour
    {   
        /// <summary>
        /// Enables or Disables the Oscillating effect of this Component.
        /// </summary>
        public bool ShouldOscillate = true;

        /// <summary>
        /// The TargetColor that this Component should oscillate to.
        /// </summary>
        public Color TargetColor = Color.white;

        /// <summary>
        /// Interval of oscillation.
        /// </summary>
        public float Interval = 1;

        private bool started = false;
        private GraphicOrRenderer m_target = null;
        private Color startColor;

        /// <summary>
        /// Return the current color of the attached graphic to it's initial color.
        /// </summary>
        public void ResetColor()
        {
            m_target.SetColor(startColor);
        }

        /// <summary>
        /// Sets a new initial color.
        /// </summary>
        /// <param name="startColor">The color value to be set as the initial color.</param>
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
                m_target = new GraphicOrRenderer(GetComponent<Graphic>(),
                                                 GetComponent<Renderer>());

                if(m_target.HasAnyReference())
                {
                    startColor = m_target.GetColor();

                    CoroutineRunner.RunCoroutine(Run());
                }
                else
                {
                    Debug.LogError($"Failed to find target in {gameObject.name} to apply color!");
                }
            }
        }

        private IEnumerator Run()
        {
            while(true)
            {
                // Return color back to the initial color
                ResetColor();
                
                // Wait until this Component is enabled, ShouldOscillate is true and Interval is a positive value
                if(!enabled || !ShouldOscillate || Interval <= 0)
                    yield return new WaitUntil(() => enabled && ShouldOscillate && Interval > 0);

                float halfInterval = Interval/2;
                float accumulatedTime = 0;

                // Lerp Graphic's color from the startColor to the targetColor in halfInterval seconds
                while(accumulatedTime < halfInterval)
                {
                    accumulatedTime += Time.deltaTime;
                    
                    m_target.SetColor(Color.Lerp(startColor, TargetColor, accumulatedTime/halfInterval));

                    yield return null;
                }

                accumulatedTime = 0;

                // Lerp Graphic's color from the targetColor to the startColor in halfInterval seconds
                while(accumulatedTime < halfInterval)
                {
                    accumulatedTime += Time.deltaTime;

                    m_target.SetColor(Color.Lerp(TargetColor, startColor, accumulatedTime/halfInterval));

                    yield return null;
                }
            }   
        }

        /// <summary>
        /// Store a Graphic, Sprite Renderer and Mesh Renderer and provide methods to access and modify their color values.
        /// </summary>
        private class GraphicOrRenderer
        {
            /// <summary>
            /// Set color of all stored references of graphic or renderer.
            /// </summary>
            public Action<Color> SetColor = null;

            /// <summary>
            /// Returns one of the colors of the stored references in the following priority order:
            /// <para>Renderer</para>
            /// <para>Graphic</para>
            /// </summary>
            public Func<Color> GetColor = null;

            private Graphic m_graphic = null;
            private Renderer m_rend = null;

            /// <summary>
            /// Initializes a GraphicOrRenderer object, storing the passed references and defining the SetColor and GetColor calls
            /// </summary>
            /// <param name="graphic">The Graphic component reference to be stored and used by the GraphicOrRenderer Object.</param>
            /// <param name="renderer">The Renderer component reference to be stored and used by the GraphicOrRenderer Object.</param>
            public GraphicOrRenderer(Graphic graphic, Renderer renderer)
            {
                SetColor = null;

                m_graphic = graphic;
                m_rend = renderer;

                if(m_graphic)
                {
                    SetColor += (c) => m_graphic.color = c;
                    GetColor = () => m_graphic.color;
                }
                
                if(m_rend)
                {
                    SetColor += (c) => m_rend.material.color = c;
                    GetColor = () => m_rend.material.color;
                }        
            }

            /// <summary>
            /// Returns true if a Graphic or a Renderer is stored, otherwise false.
            /// </summary>
            public bool HasAnyReference()
            {
                return m_graphic != null || m_rend != null;
            }
        }
    }
}