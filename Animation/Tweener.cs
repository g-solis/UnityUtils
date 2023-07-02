using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils.Core;
using Utils.Extension;

namespace Utils.Animation
{
    public class Tweener
    {
        public enum TweenType
        {
            /// <summary>
            /// Linear Curve.
            /// </summary>
            Linear,

            /// <summary>
            /// InOutSine Curve.
            /// </summary>
            Smooth,

            /// <summary>
            /// InCubic Curve.
            /// </summary>
            SmoothStart,

            /// <summary>
            /// OutCubic Curve.
            /// </summary>
            SmoothEnd,

            /// <summary>
            /// InOutCubic Curve.
            /// </summary>
            ExtraSmooth,

            /// <summary>
            /// OutBack Curve.
            /// </summary>
            Elastic
        }

        /// <summary>
        /// Tweens a float from it's current value to the given final value in the given duration.
        /// </summary>
        /// <param name="getter">The Function to get the Color current value.</param>
        /// <param name="setter">The Action to set the Color value during the tweening.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        /// <param name="finalValue">The target final value for the Tween.</param>
        public static Tween TweenValue(Func<float> getter, Action<float> setter, float finalValue, float duration)
        {
            return Tween.StartTween<float>(getter, setter, finalValue, duration);
        }
    }

    public static class TweenerExtension
    {
        private static List<(UnityEngine.Object, Tween)> allCurrentTweens = new List<(UnityEngine.Object, Tween)>();

#region Transform
        /// <summary>
        /// Tweens a Transform from it's current position value to the given final position value in the given duration.
        /// </summary>
        /// <param name="finalPos">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenPos(this Transform target, Vector3 finalPos, float duration)
        {
            return StoreTween(target, Tween.StartTween<Vector3>(() => target.position, (x) => target.position = x, finalPos, duration));
        }

        /// <summary>
        /// Tweens a Transform from it's current local position value to the given final local position value in the given duration.
        /// </summary>
        /// <param name="finalPos">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenLocalPos(this Transform target, Vector3 finalPos, float duration)
        {
            return StoreTween(target, Tween.StartTween<Vector3>(() => target.localPosition, (x) => target.localPosition = x, finalPos, duration));
        }

        /// <summary>
        /// Tweens a Transform from it's current scale value to the given final scale value in the given duration.
        /// </summary>
        /// <param name="finalScale">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenScale(this Transform target, Vector3 finalScale, float duration)
        {
            return StoreTween(target, Tween.StartTween<Vector3>(() => target.localScale, (x) => target.localScale = x, finalScale, duration));
        }
#endregion

#region Color
        /// <summary>
        /// Tweens a Graphic from it's current color value to the given final color value in the given duration.
        /// </summary>
        /// <param name="finalColor">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenColor(this Graphic target, Color finalColor, float duration)
        {
            return StoreTween(target, Tween.StartTween<Color>(() => target.color, (x) => target.color = x, finalColor, duration));
        }

        /// <summary>
        /// Tweens a Renderer from it's current color value to the given final color value in the given duration.
        /// </summary>
        /// <param name="finalColor">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenColor(this Renderer target, Color finalColor, float duration)
        {
            return StoreTween(target, Tween.StartTween<Color>(() => target.material.color, (x) => target.material.color = x, finalColor, duration));
        }
#endregion

#region Alpha
        /// <summary>
        /// Tweens a Graphic from it's current alpha value to the given final alpha value in the given duration.
        /// </summary>
        /// <param name="finalAlpha">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenAlpha(this Graphic target, float finalAlpha, float duration)
        {
            return StoreTween(target, Tween.StartTween<float>(() => target.color.a, (x) => target.SetAlpha(x), finalAlpha, duration));
        }
        
        /// <summary>
        /// Tweens a Renderer from it's current alpha value to the given final alpha value in the given duration.
        /// </summary>
        /// <param name="finalAlpha">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenAlpha(this Renderer target, float finalAlpha, float duration)
        {
            return StoreTween(target, Tween.StartTween<float>(() => target.material.color.a, (x) => target.SetAlpha(x), finalAlpha, duration));
        }
        
        /// <summary>
        /// Tweens a CanvasGroup from it's current alpha value to the given final alpha value in the given duration.
        /// </summary>
        /// <param name="finalAlpha">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenAlpha(this CanvasGroup target, float finalAlpha, float duration)
        {
            return StoreTween(target, Tween.StartTween<float>(() => target.alpha, (x) => target.alpha = x, finalAlpha, duration));
        }
#endregion

#region Other
        /// <summary>
        /// Tweens a Slider from it's current slider value to the given final slider value in the given duration.
        /// </summary>
        /// <param name="finalValue">The target final value for the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween TweenSliderValue(this Slider target, float finalValue, float duration)
        {
            return StoreTween(target, Tween.StartTween<float>(() => target.value, (x) => target.value = x, finalValue, duration));
        }
#endregion

        /// <summary>
        /// Checks if there is any Tweens currently running on this Object.
        /// </summary>
        public static bool IsTweening(this UnityEngine.Object target)
        {
            if(allCurrentTweens.Find((p) => p.Item1 == target).Item2 != null)
                return true;
            
            return false;
        }

        /// <summary>
        /// Kill all Tweens currently running on this Object.
        /// </summary>
        public static void KillAllTweens(this UnityEngine.Object target)
        {
            var allTweens = allCurrentTweens.FindAll((p) => p.Item1 == target);

            if(allTweens != null)
                foreach((UnityEngine.Object, Tween) p in allTweens)
                {
                    if(p.Item2 != null)
                        p.Item2.Kill();
                }
        }

        /// <summary>
        /// Kill all Tweens from this list.
        /// </summary>
        public static void KillTweens(this List<Tween> list)
        {
            if(list != null)
            {
                // Clone list to avoid changing the original list mid-foreach
                IEnumerable<Tween> clonedList = list.Clone();

                foreach(Tween t in clonedList)
                {
                    if(t != null)
                        t.Kill();
                }

                list.Clear();
            }
        }

        private static Tween StoreTween(UnityEngine.Object target, Tween t)
        {
            allCurrentTweens.Add((target, t));
            t.onComplete += () => allCurrentTweens.Remove((target, t));

            return t;
        }
    }

    public class Tween
    {
        /// <summary>
        /// Defines if the Tween should be paused or not.
        /// </summary>
        public bool isPaused = false;

        /// <summary>
        /// Called when the Tween starts.
        /// </summary>
        public System.Action onTweenStart =         new System.Action(() => {});

        /// <summary>
        /// Called every time the Tween updates.
        /// </summary>
        public System.Action<float> onTweenUpdate = new System.Action<float>((x) => {});
        
        /// <summary>
        /// Called when the tween finishes.
        /// </summary>
        public System.Action onTweenFinish =        new System.Action(() => {});

        /// <summary>
        /// Called when the Tween finishes or is killed.
        /// </summary>
        public System.Action onComplete =           new System.Action(() => {});

        /// <summary>
        /// Called when the Tween is killed.
        /// </summary>
        public System.Action onKill =               new System.Action(() => {});

        public float Duration {
            get
            {
                return duration;
            }
        }

        private Coroutine coroutine;
        private Tweener.TweenType tweenType = Tweener.TweenType.Linear;
        private Func<float, float> tweenCurve;
        private bool useUnscaledTime = false;
        private float delay = 0;
        private float duration = 0;
        
        /// <summary>
        /// Starts a Tween.
        /// </summary>
        /// <param name="getter">The Function that the Tween should use to get the starting value of the target.</param>
        /// <param name="setter">The Action that the Tween should use to set the value of the target.</param>
        /// <param name="finalValue">The Final value of the Tween.</param>
        /// <param name="duration">The Duration of the Tween.</param>
        public static Tween StartTween<T>(Func<T> getter, Action<T> setter, T finalValue, float duration)
        {
            Tween t = new Tween();

            t.coroutine = CoroutineRunner.RunCoroutine(t.TweenCoroutine(getter, setter, finalValue));
            t.duration = duration;
            t.tweenCurve = getTweenCurve(t.tweenType);

            return t;
        }

        /// <summary>
        /// Sets the Tween Type of the Tween.
        /// </summary>
        /// <param name="tweenType">The TweenType to be set.</param>
        public Tween SetTweenType(Tweener.TweenType tweenType)
        {
            this.tweenType = tweenType;
            tweenCurve = getTweenCurve(tweenType);
            return this;
        }

        /// <summary>
        /// Sets the starting delay of the Tween.
        /// </summary>
        /// <param name="delay">The starting delay to be set, in seconds.</param>
        public Tween SetDelay(float delay)
        {
            this.delay = Mathf.Max(delay, 0);
            return this;
        }

        /// <summary>
        /// Sets if the Tween should use Unscaled Time.
        /// </summary>
        /// <param name="shouldUseUnscaledTime">Should the Tween use Unscaled Time?</param>
        public Tween UseUnscaledTime(bool shouldUseUnscaledTime)
        {
            useUnscaledTime = shouldUseUnscaledTime;
            return this;
        }

        /// <summary>
        /// Unpauses the Tween.
        /// </summary>
        public Tween UnPause()
        {
            isPaused = false;
            return this;
        }

        /// <summary>
        /// Pauses the Tween.
        /// </summary>
        public Tween Pause()
        {
            isPaused = true;
            return this;
        }

        /// <summary>
        /// Adds the Tween to the given list. The Tween will remove itself from the said list when it completes.
        /// </summary>
        /// <param name="list">The List that the Tween should link to.</param>
        public Tween LinkToList(List<Tween> list)
        {
            if(list != null)
            {
                list.Add(this);

                onComplete += () => list.Remove(this);
            }
            return this;
        }

        /// <summary>
        /// Kills the Tween.
        /// </summary>
        public void Kill()
        {
            if(onKill != null)
                onKill.Invoke();
            
            if(onComplete != null)
                onComplete.Invoke();
            
            CoroutineRunner.StopMyCoroutine(coroutine);
        }

        private static Func<float, float> getTweenCurve(Tweener.TweenType t)
        {
            switch (t)
            {
                case Tweener.TweenType.Linear:
                    return AnimCurves.Linear;

                case Tweener.TweenType.Smooth:
                    return AnimCurves.InOutSine;
                
                case Tweener.TweenType.SmoothStart:
                    return AnimCurves.InCubic;

                case Tweener.TweenType.SmoothEnd:
                    return AnimCurves.OutCubic;

                case Tweener.TweenType.ExtraSmooth:
                    return AnimCurves.InOutCubic;

                case Tweener.TweenType.Elastic:
                    return AnimCurves.OutBack;
            }

            Debug.LogError($"Animation curve function not defined for this Enum type ({t.ToString()}).");
            return AnimCurves.Linear;
        }
        
        private IEnumerator TweenCoroutine<T>(Func<T> getter, Action<T> setter, T finalValue)
        {
            Type typeOfT = typeof(T);

            // Wait a frame, so the caller can add extra configurations before the tween starts
            yield return null;

            // Ends the coroutine if the duration is 0 or less than 0
            if(duration <= 0)
            {
                setter(finalValue);
                yield break;
            }

            // Prepare Lerp function
            #region PrepareLerpFunction
            Func<T, T, float, T> LerpFunc = (start, end, v) => {
                Debug.LogError($"Tweener couldn't find type {typeOfT.FullName} to build Lerp function!");
                return default(T);
            };

            if(typeOfT == typeof(float))
            {
                LerpFunc = (start, end, v) => (T)(object) Lerp((float)(object) start, (float)(object) end, v);
            }

            if(typeOfT == typeof(Vector2))
            {
                LerpFunc = (start, end, v) => (T)(object) Lerp((Vector2)(object) start, (Vector2)(object) end, v);
            }

            if(typeOfT == typeof(Vector3))
            {
                LerpFunc = (start, end, v) => (T)(object) Lerp((Vector3)(object) start, (Vector3)(object) end, v);
            }

            if(typeOfT == typeof(Color))
            {
                LerpFunc = (start, end, v) => (T)(object) Lerp((Color)(object) start, (Color)(object) end, v);
            }
            #endregion

            Func<float> getDeltaTime = () => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            // Wait for delay
            if(delay > 0)
            {
                float delayedTime = 0;

                while(delayedTime < delay)
                {
                    if(!isPaused)
                        delayedTime += getDeltaTime();
                    yield return null;
                }
            }

            // Wait while it's paused
            if(isPaused)
                yield return new WaitWhile(() => isPaused);

            if(onTweenStart != null)
                onTweenStart.Invoke();

            T initialValue = getter();

            float accumulatedTime = 0;

            // Animates the value
            while(duration > accumulatedTime)
            {
                if(!isPaused)
                {
                    accumulatedTime += getDeltaTime();

                    float progress = accumulatedTime/duration;

                    setter(LerpFunc(initialValue, finalValue, tweenCurve(progress)));

                    if(onTweenUpdate != null)
                        onTweenUpdate.Invoke(progress);
                }
                yield return null;
            }

            // Sets the final value
            setter(finalValue);

            if(onTweenFinish != null)
                onTweenFinish.Invoke();

            if(onComplete != null)
                onComplete.Invoke();
        }

#region Lerps

        private float Lerp(float initialValue, float finalValue, float v)
        {
            return Mathf.Lerp(initialValue, finalValue, v);
        }

        private Vector2 Lerp(Vector2 initialValue, Vector2 finalValue, float v)
        {
            Vector2 vect = finalValue - initialValue;
            float delta = vect.magnitude;
            vect = vect.normalized;

            return v * delta * vect + initialValue;
        }

        private Vector3 Lerp(Vector3 initialValue, Vector3 finalValue, float v)
        {
            Vector3 vect = finalValue - initialValue;
            float delta = vect.magnitude;
            vect = vect.normalized;

            return v * delta * vect + initialValue;
        }

        private Color Lerp(Color initialValue, Color finalValue, float v)
        {
            return Color.Lerp(initialValue, finalValue, v);
        }
#endregion  

        public static implicit operator bool(Tween t) => t != null;
    }
}
