using UnityEngine;

namespace Utils
{
    public static class ColorExtensions
    {

#region Renderer

        /// <summary>
        /// Sets the alpha value of the Renderer's material color.
        /// </summary>
        /// <param name="alpha">The alpha value to set the material color to, clamped between 0 and 1.</param>
        public static void SetAlpha(this Renderer rend, float alpha)
        {
            Color c = rend.material.color;
            c.a = Mathf.Clamp01(alpha);
            rend.material.color = c;
        }

        /// <summary>
        /// Returns the Renderer's material color with the same original RGB values, but with the specified alpha. 
        /// </summary>
        /// <param name="alpha">The alpha value of the returned color, clamped between 0 and 1.</param>
        public static Color GetColorWithSetAlpha(this Renderer rend, float alpha)
        {
            Color c = rend.material.color;
            c.a = Mathf.Clamp01(alpha);
            return c;
        }

#endregion

#region Graphic
        
        /// <summary>
        /// Sets the alpha value of the Graphic's color.
        /// </summary>
        /// <param name="alpha">The alpha value to set the color to, clamped between 0 and 1.</param>
        public static void SetAlpha(this UnityEngine.UI.Graphic g, float alpha)
        {
            Color c = g.color;
            c.a = Mathf.Clamp01(alpha);
            g.color = c;
        }

        /// <summary>
        /// Returns the Renderer's material color with the same original RGB values, but with the specified alpha. 
        /// </summary>
        /// <param name="alpha">The alpha value of the returned color, clamped between 0 and 1.</param>
        public static Color GetColorWithSetAlpha(this UnityEngine.UI.Graphic g, float alpha)
        {
            Color c = g.color;
            c.a = Mathf.Clamp01(alpha);
            return c;
        }

#endregion

    }
}