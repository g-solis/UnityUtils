using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class Tweener
    {
        public enum TweenType
        {
            Linear,
            Smooth,
            ExtraSmooth,
            Elastic
        }

        /// <summary>
        /// Coroutine to Tween a float from it's current value to the given final value in the given duration.
        /// </summary>
        /// <param name="getter">The Function to get the float current value.</param>
        /// <param name="setter">The Action to set the float value during the tweening.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="finalValue">The target final value for the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static IEnumerator TweenCoroutine(Func<float> getter, Action<float> setter, float finalValue, float duration, TweenType tweenType, bool useUnscaledTime = false)
        {
            // Ends the coroutine if the duration is 0 or less than 0
            if(duration <= 0)
            {
                setter(finalValue);
                yield break;
            }

            float initialValue = getter();
            Func<float, float> tweenCurve = getTweenCurve(tweenType);
            Func<float> getDeltaTime = useUnscaledTime ? () => Time.unscaledDeltaTime : () => Time.deltaTime;

            float accumulatedTime = 0;

            // Animates the value
            while(duration > accumulatedTime)
            {
                accumulatedTime += getDeltaTime();

                setter(Mathf.Lerp(initialValue, finalValue, tweenCurve(accumulatedTime/duration)));

                yield return null;
            }

            // Sets the final value
            setter(finalValue);
        }

        /// <summary>
        /// Coroutine to Tween a Vector2 from it's current value to the given final value in the given duration.
        /// </summary>
        /// <param name="getter">The Function to get the Vector2 current value.</param>
        /// <param name="setter">The Action to set the Vector2 value during the tweening.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="finalValue">The target final value for the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static IEnumerator TweenCoroutine(Func<Vector2> getter, Action<Vector2> setter, Vector2 finalValue, float duration, TweenType tweenType, bool useUnscaledTime = false)
        {
            // Ends the coroutine if the duration is 0 or less than 0
            if(duration <= 0)
            {
                setter(finalValue);
                yield break;
            }

            // Calculates the required variables (delta and vect) to calculate the current value of the Lerp
            Vector2 initialValue = getter();
            Vector2 vect = finalValue - initialValue;
            float delta = vect.magnitude;
            vect = vect.normalized;

            Func<float, float> tweenCurve = getTweenCurve(tweenType);
            Func<float> getDeltaTime = useUnscaledTime ? () => Time.unscaledDeltaTime : () => Time.deltaTime;

            float accumulatedTime = 0;

            // Animates the value
            while(duration > accumulatedTime)
            {
                accumulatedTime += getDeltaTime();

                // Implementing my own Lerp to allow for curve extrapolation
                setter(tweenCurve(accumulatedTime/duration) * delta * vect + initialValue);

                yield return null;
            }

            // Sets the final value
            setter(finalValue);
        }

        /// <summary>
        /// Coroutine to Tween a Vector3 from it's current value to the given final value in the given duration.
        /// </summary>
        /// <param name="getter">The Function to get the Vector3 current value.</param>
        /// <param name="setter">The Action to set the Vector3 value during the tweening.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="finalValue">The target final value for the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static IEnumerator TweenCoroutine(Func<Vector3> getter, Action<Vector3> setter, Vector3 finalValue, float duration, TweenType tweenType, bool useUnscaledTime = false)
        {
            // Ends the coroutine if the duration is 0 or less than 0
            if(duration <= 0)
            {
                setter(finalValue);
                yield break;
            }

            // Calculates the required variables (delta and vect) to calculate the current value of the Lerp
            Vector3 initialValue = getter();
            Vector3 vect = finalValue - initialValue;
            float delta = vect.magnitude;
            vect = vect.normalized;

            Func<float, float> tweenCurve = getTweenCurve(tweenType);
            Func<float> getDeltaTime = useUnscaledTime ? () => Time.unscaledDeltaTime : () => Time.deltaTime;

            float accumulatedTime = 0;

            // Animates the value
            while(duration > accumulatedTime)
            {
                accumulatedTime += getDeltaTime();

                // Implementing my own Lerp to allow for curve extrapolation
                setter(tweenCurve(accumulatedTime/duration) * delta * vect + initialValue);

                yield return null;
            }
            
            // Sets the final value
            setter(finalValue);
        }
        
        /// <summary>
        /// Coroutine to Tween a Color from it's current value to the given final value in the given duration.
        /// </summary>
        /// <param name="getter">The Function to get the Color current value.</param>
        /// <param name="setter">The Action to set the Color value during the tweening.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="finalValue">The target final value for the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static IEnumerator TweenCoroutine(Func<Color> getter, Action<Color> setter, Color finalValue, float duration, TweenType tweenType, bool useUnscaledTime = false)
        {
            // Ends the coroutine if the duration is 0 or less than 0
            if(duration <= 0)
            {
                setter(finalValue);
                yield break;
            }

            Color initialValue = getter();
            Func<float, float> tweenCurve = getTweenCurve(tweenType);
            Func<float> getDeltaTime = useUnscaledTime ? () => Time.unscaledDeltaTime : () => Time.deltaTime;

            float accumulatedTime = 0;

            // Animates the value
            while(duration > accumulatedTime)
            {
                accumulatedTime += getDeltaTime();

                setter(Color.Lerp(initialValue, finalValue, tweenCurve(accumulatedTime/duration)));

                yield return null;
            }

            // Sets the final value
            setter(finalValue);
        }

        /// <summary>
        /// Tweens a float from it's current value to the given final value in the given duration.
        /// </summary>
        /// <param name="getter">The Function to get the Color current value.</param>
        /// <param name="setter">The Action to set the Color value during the tweening.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="finalValue">The target final value for the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenValue(Func<float> getter, Action<float> setter, float finalValue, float duration, TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(TweenCoroutine(getter, setter, finalValue, duration, tweenType, useUnscaledTime));
        }

        /// <summary>
        /// Stop a given Coroutine that were started by the Tweener class, always use this method to stop Coroutines that were started by the Tweener.
        /// </summary>
        /// <param name="coroutine">The coroutine to be stopped.</param>
        public static void StopTweening(Coroutine coroutine)
        {
            if(coroutine != null)
                CoroutineRunner.StopMyCoroutine(coroutine);
        }

        // Returns the matching Animation Curve of the given TweenType
        private static Func<float, float> getTweenCurve(TweenType t)
        {
            switch (t)
            {
                case TweenType.Linear:
                    return AnimCurves.Linear;

                case TweenType.Smooth:
                    return AnimCurves.InOutSine;

                case TweenType.ExtraSmooth:
                    return AnimCurves.InOutCubic;

                case TweenType.Elastic:
                    return AnimCurves.OutBack;
            }

            Debug.LogError($"Animation curve function not defined for this Enum type ({t.ToString()}).");
            return AnimCurves.Linear;
        }
    }

    public static class TweenerExtension
    {
#region Transform
        /// <summary>
        /// Tweens a Transform from it's current position value to the given final position value in the given duration.
        /// </summary>
        /// <param name="finalPos">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenPos(this Transform target, Vector3 finalPos, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.position, (x) => target.position = x, finalPos, duration, tweenType, useUnscaledTime));
        }

        /// <summary>
        /// Tweens a Transform from it's current local position value to the given final local position value in the given duration.
        /// </summary>
        /// <param name="finalPos">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenLocalPos(this Transform target, Vector3 finalPos, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.localPosition, (x) => target.localPosition = x, finalPos, duration, tweenType, useUnscaledTime));
        }

        /// <summary>
        /// Tweens a Transform from it's current scale value to the given final scale value in the given duration.
        /// </summary>
        /// <param name="finalScale">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenScale(this Transform target, Vector3 finalScale, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.localScale, (x) => target.localScale = x, finalScale, duration, tweenType, useUnscaledTime));
        }
#endregion

#region Color
        /// <summary>
        /// Tweens a Graphic from it's current color value to the given final color value in the given duration.
        /// </summary>
        /// <param name="finalColor">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenColor(this Graphic target, Color finalColor, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.color, (x) => target.color = x, finalColor, duration, tweenType, useUnscaledTime));
        }

        /// <summary>
        /// Tweens a Renderer from it's current color value to the given final color value in the given duration.
        /// </summary>
        /// <param name="finalColor">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenColor(this Renderer target, Color finalColor, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.material.color, (x) => target.material.color = x, finalColor, duration, tweenType, useUnscaledTime));
        }
#endregion

#region Alpha
        /// <summary>
        /// Tweens a Graphic from it's current alpha value to the given final alpha value in the given duration.
        /// </summary>
        /// <param name="finalAlpha">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenAlpha(this Graphic target, float finalAlpha, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.color.a, (x) => target.SetAlpha(x), finalAlpha, duration, tweenType, useUnscaledTime));
        }
        
        /// <summary>
        /// Tweens a Renderer from it's current alpha value to the given final alpha value in the given duration.
        /// </summary>
        /// <param name="finalAlpha">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenAlpha(this Renderer target, float finalAlpha, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.material.color.a, (x) => target.SetAlpha(x), finalAlpha, duration, tweenType, useUnscaledTime));
        }
        
        /// <summary>
        /// Tweens a CanvasGroup from it's current alpha value to the given final alpha value in the given duration.
        /// </summary>
        /// <param name="finalAlpha">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="tweenType">The Tween Curve that should be used during the Tween.</param>
        /// <param name="useUnscaledTime">Should the Tween use Unscaled Delta Time?.</param>
        public static Coroutine TweenAlpha(this CanvasGroup target, float finalAlpha, float duration, Tweener.TweenType tweenType, bool useUnscaledTime = false)
        {
            return CoroutineRunner.RunCoroutine(Tweener.TweenCoroutine(() => target.alpha, (x) => target.alpha = x, finalAlpha, duration, tweenType, useUnscaledTime));
        }
#endregion
    }
}
