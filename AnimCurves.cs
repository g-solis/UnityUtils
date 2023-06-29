using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class AnimCurves
    {
        /// <summary>
        /// Returns the result of the Linear curve value of X
        /// <para>Formula: Y = X </para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float Linear(float x)
        {
            return Mathf.Clamp01(x);
        }

        /// <summary>
        /// Returns the result of the InOutSine curve value of X
        /// <para>Formula: Y = -(cos(PI*X) - 1) / 2 </para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float InOutSine(float x)
        {
            return -(Mathf.Cos(Mathf.PI * Mathf.Clamp01(x)) - 1) / 2;
        }

        /// <summary>
        /// Returns the result of the InOutCubic curve value of X
        /// <para>Formula: Y = 4*X^3 (for values under 0.5) or 1 - (-2*X + 2)^3 / 2 (for values over or equal to 0.5)</para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float InOutCubic(float x)
        {
            x = Mathf.Clamp01(x);

            return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
        }

        /// <summary>
        /// Returns the result of the InCubic curve value of X
        /// <para>Formula: Y = X^3</para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float InCubic(float x)
        {
            x = Mathf.Clamp01(x);

            return x * x * x;
        }

        /// <summary>
        /// Returns the result of the OutCubic curve value of X
        /// <para>Formula: Y = 1 - (1 - X)^3</para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float OutCubic(float x)
        {
            return 1 - Mathf.Pow(1 - Mathf.Clamp01(x), 3);
        }
        
        /// <summary>
        /// Returns the result of the OutBack curve value of X
        /// <para>Formula: 1 + c3 * (x - 1)^3 + c1 * (x - 1)^2</para>
        /// <para>Where: </para>
        /// <para>c1 = 1.70158 </para>
        /// <para>c3 = 2.70158 </para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float OutBack(float x)
        {
            x = Mathf.Clamp01(x);
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
        }

        /// <summary>
        /// Returns the result of the Mountain curve value of X
        /// <para>This curve starts and ends in 0</para>
        /// <para>Formula: sin(x * pi)</para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float Mountain(float x)
        {
            return Mathf.Sin(Mathf.Clamp01(x) * Mathf.PI);
        }

        /// <summary>
        /// Returns the result of the Mountain curve value of X
        /// <para>This curve starts and ends in 0</para>
        /// <para>Formula: (sin((x - 0.25) * 2 * pi) + 1) / 2</para>
        /// </summary>
        /// <param name="x">The value to apply the curve to.</param>
        public static float MountainWithEase(float x)
        {
            return (Mathf.Sin((Mathf.Clamp01(x) - 0.25f) * 2 * Mathf.PI) + 1) / 2;
        }
    }
}
