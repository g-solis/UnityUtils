using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteExtensions
{
    /// <summary>
    /// A Struct to store each corner and the center of the SpriteRenderer.
    /// </summary>
    public struct NamedSprRendererPositions
    {
        public Vector3 center;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 topCenter;
        public Vector3 bottomCenter;
    }

    /// <summary>
    /// Gets the positions of each corner and the center of the SpriteRenderer.
    /// </summary>
    public static NamedSprRendererPositions GetPositions(this SpriteRenderer target)
    {
        Bounds bounds = target.bounds;

        NamedSprRendererPositions positions = new NamedSprRendererPositions();

        var center = bounds.center;
        positions.center = center;
        
        var topRight = bounds.max;
        positions.topRight = topRight;
        
        var bottomLeft = bounds.min;
        positions.bottomLeft = bottomLeft;
        
        var topCenter = topRight;
        topCenter.x = center.x;
        positions.topCenter = topCenter;
        
        var bottomCenter = bottomLeft;
        bottomCenter.x = center.x;
        positions.bottomCenter = bottomCenter;

        return positions;
    }
}
