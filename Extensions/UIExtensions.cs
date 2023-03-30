using UnityEngine;

namespace Utils
{
    public static class UIExtensions
    {
        /// <summary>
        /// Set the Height of the RectTransform while respecting it's anchors.
        /// </summary>
        /// <param name="height">The Height value to be set.</param>
        public static void SetHeight(this RectTransform value, float height)
        {
            value.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            value.ForceUpdateRectTransforms();
        }

        /// <summary>
        /// Set the Width of the RectTransform while respecting it's anchors.
        /// </summary>
        /// <param name="width">The Width value to be set.</param>
        public static void SetWidth(this RectTransform value, float width)
        {
            value.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            value.ForceUpdateRectTransforms();
        }
    }
}

