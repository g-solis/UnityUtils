using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils.Animation;
using Utils.Extension;

namespace Utils.UI
{
    /// <summary>
    /// This is a functional Window implementation that can be inherited to override some of it's defined behavior. It requires DoTween to function.
    /// <para>The methods that can be overridden to implement extra behavior are the following:</para>
    /// <para>CustomModalClick()</para>
    /// <para>OnInit()</para>
    /// <para>BeforeOpen()</para>
    /// <para>AfterOpen()</para>
    /// <para>BeforeClose()</para>
    /// <para>AfterClose()</para>
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class IWindow : MonoBehaviour
    {
        public enum AnimationType
        {
            Scale,
            Fade,
            SlideUp,
            SlideDown,
            SlideLeft,
            SlideRight
        }

        public enum ModalInteraction
        {
            Nothing,
            CloseWindow,
            Custom
        }

        [Header("Window Parameters")]
        [Tooltip("The type of animation used when opening the window.")]
        [SerializeField] protected AnimationType openAnimation;

        [Tooltip("The animation easing that the opening animation should use.")]
        [SerializeField] protected Tweener.TweenType openCurve;

        [Tooltip("The duration of the opening animation.")]
        [SerializeField] protected float openDuration = 1;

        [Tooltip("The type of animation used when closing the window.")]
        [SerializeField] protected AnimationType closeAnimation;
        
        [Tooltip("The animation easing that the closing animation should use.")]
        [SerializeField] protected Tweener.TweenType closeCurve;

        [Tooltip("The duration of the closing animation.")]
        [SerializeField] protected float closeDuration = 1;


        [Header("Modal Parameters")]
        [SerializeField] protected bool shouldUseModal = true;

        [Tooltip("The color the modal should use.")]
        [SerializeField] protected Color modalColor = new Color(0, 0, 0, 0.5f);

        [Tooltip("What should happen when the modal is clicked.")]
        [SerializeField] protected ModalInteraction modalOnClick;


        [Header("Window References")]
        [Tooltip("The RectTransform of the Window.")]
        [SerializeField] protected RectTransform windowRect;

        [Tooltip("The buttons that should be disabled while the opening or closing animation is playing.")]
        [SerializeField] protected List<Button> DisableWhileAnim = new List<Button>();

        [Tooltip("The buttons that should be disabled while the window is open.")]
        [SerializeField] protected List<Button> DisableWhileOpen = new List<Button>();
        [Space(20)]

        private RectTransform m_rectTransform;
        private Vector3 startPos;
        private float startAlpha;
        private Vector3 startScale;
        private Image modal;
        private float modalStartAlpha;
        private CanvasGroup canvasGroup;

        private Coroutine currentCoroutine = null;
        private Tween windowTween = null;
        private Tween modalTween = null;
        private Tween canvasTween = null;

        private bool initted = false;

#region VirtualMethods
        /// <summary>
        /// Called when the modal is clicked while the CustomModalClick interaction is set on modalOnClick.
        /// </summary>
        protected virtual void CustomModalClick() {}
        /// <summary>
        /// Called when Window is initialized.
        /// </summary>
        protected virtual void OnInit() {}
        /// <summary>
        /// Called before the Window opens.
        /// </summary>
        protected virtual void BeforeOpen() {}
        /// <summary>
        /// Called after the Window opens.
        /// </summary>
        protected virtual void AfterOpen() {}
        /// <summary>
        /// Called before the Window closes.
        /// </summary>
        protected virtual void BeforeClose() {}
        /// <summary>
        /// Called after the Window closes.
        /// </summary>
        protected virtual void AfterClose() {}
#endregion

        /// <summary>
        /// Opens the Window, interrupting any animation that was playing previously.
        /// </summary>
        public void Open()
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                
                if(windowTween)
                    windowTween.Kill();

                if(shouldUseModal && modalTween)
                        modalTween.Kill();

                if(canvasTween)
                    canvasTween.Kill();
            }
            
            currentCoroutine = StartCoroutine(OpenCoroutine());
        }

        /// <summary>
        /// Closes the Window, interrupting any animation that was playing previously.
        /// </summary>
        public void Close()
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                
                if(windowTween)
                    windowTween.Kill();

                if(shouldUseModal && modalTween)
                        modalTween.Kill();

                if(canvasTween)
                    canvasTween.Kill();
            }
            
            currentCoroutine = StartCoroutine(CloseCoroutine());
        }

        /// <summary>
        /// Called when the Modal is clicked
        /// </summary>
        public void ModalClick()
        {
            switch(modalOnClick)
            {
                case ModalInteraction.CloseWindow:
                    Close();
                    break;

                case ModalInteraction.Custom:
                    CustomModalClick();
                    break;
            }
        }


        protected IEnumerator OpenCoroutine()
        {
            // Set the Window and Modal to their initial state and initializes the Window if it isn't initialized
            windowRect.gameObject.SetActive(true);

            if(shouldUseModal)
                modal.gameObject.SetActive(true);

            Init();
            BeforeOpen();
            
            windowRect.localPosition = startPos;
            windowRect.localScale = startScale;
            canvasGroup.alpha = startAlpha;

            if(shouldUseModal)
                modal.SetAlpha(0);
            
            List<Vector2> outSidePositions = GetOutSidePositions();
            
            // Disable buttons that should be disable while the window is open
            SetButtonsWhileOpen(false);

            // Updates the Window to the initial state of the animation and starts it
            switch(openAnimation)
            {
                case AnimationType.Scale:
                    windowRect.localScale = Vector3.zero;
                    yield return null;
                    windowTween = windowRect.TweenScale(startScale, openDuration)
                        .SetTweenType(openCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.Fade:
                    canvasGroup.alpha = 0;
                    yield return null;
                    canvasTween = canvasGroup.TweenAlpha(startAlpha, openDuration)
                        .SetTweenType(openCurve)
                        .UseUnscaledTime(true);
                    canvasTween.onComplete += () => canvasTween = null;
                    break;

                case AnimationType.SlideUp:
                    windowRect.localPosition = outSidePositions[0];
                    yield return null;
                    windowTween = windowRect.TweenLocalPos(startPos, openDuration)
                        .SetTweenType(openCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.SlideDown:
                    windowRect.localPosition = outSidePositions[1];
                    yield return null;
                    windowTween = windowRect.TweenLocalPos(startPos, openDuration)
                        .SetTweenType(openCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.SlideLeft:
                    windowRect.localPosition = outSidePositions[2];
                    yield return null;
                    windowTween = windowRect.TweenLocalPos(startPos, openDuration)
                        .SetTweenType(openCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.SlideRight:
                    windowRect.localPosition = outSidePositions[3];
                    yield return null;
                    windowTween = windowRect.TweenLocalPos(startPos, openDuration)
                        .SetTweenType(openCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;
            }

            // Start modal fade animation
            if(shouldUseModal)
            {
                modalTween = modal.TweenAlpha(modalStartAlpha, openDuration)
                    .UseUnscaledTime(true);
                modalTween.onComplete += () => modalTween = null;
            }


            // Disable buttons that should be disabled while the animation is playing
            SetButtonsAnim(false);

            yield return new WaitForSecondsRealtime(openDuration);
            
            // Enable buttons that should be disabled once animation stops playing
            SetButtonsAnim(true);

            currentCoroutine = null;
            windowTween = null;
            modalTween = null;
            canvasTween = null;
            AfterOpen();
        }

        protected IEnumerator CloseCoroutine()
        {
            // Set the Window and Modal to their initial state
            BeforeClose();

            windowRect.localPosition = startPos;
            windowRect.localScale = startScale;
            canvasGroup.alpha = startAlpha;

            if(shouldUseModal)
                modal.SetAlpha(modalStartAlpha);
            
            yield return null;

            List<Vector2> outSidePositions = GetOutSidePositions();

            // Start the animation
            switch(closeAnimation)
            {
                case AnimationType.Scale:
                    windowTween = windowRect.TweenScale(Vector3.zero, closeDuration)
                        .SetTweenType(closeCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.Fade:
                    canvasTween = canvasGroup.TweenAlpha(0, closeDuration)
                        .SetTweenType(closeCurve)
                        .UseUnscaledTime(true);
                    canvasTween.onComplete += () => canvasTween = null;
                    break;

                case AnimationType.SlideUp:
                    windowTween = windowRect.TweenLocalPos(outSidePositions[0], closeDuration)
                        .SetTweenType(closeCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.SlideDown:
                    windowTween = windowRect.TweenLocalPos(outSidePositions[1], closeDuration)
                        .SetTweenType(closeCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.SlideLeft:
                    windowTween = windowRect.TweenLocalPos(outSidePositions[2], closeDuration)
                        .SetTweenType(closeCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;

                case AnimationType.SlideRight:
                    windowTween = windowRect.TweenLocalPos(outSidePositions[3], closeDuration)
                        .SetTweenType(closeCurve)
                        .UseUnscaledTime(true);
                    windowTween.onComplete += () => windowTween = null;
                    break;
            }

            // Start modal fade animation
            if(shouldUseModal)
            {
                modalTween = modal.TweenAlpha(0, closeDuration)
                    .UseUnscaledTime(true);
                modalTween.onComplete += () => modalTween = null;
            }

            // Disable buttons that should be disabled while the animation is playing
            SetButtonsAnim(false);

            yield return new WaitForSecondsRealtime(closeDuration);

            // Enable buttons that should be disabled once animation stops playing
            SetButtonsAnim(true);

            currentCoroutine = null;
            windowTween = null;
            modalTween = null;
            canvasTween = null;
            // Enable buttons that should be disable while the window is open
            SetButtonsWhileOpen(true);
            AfterClose();

            windowRect.gameObject.SetActive(false);

            if(shouldUseModal)
                modal.gameObject.SetActive(false);
        }

        protected void Start()
        {
            Init();

            windowRect.gameObject.SetActive(false);

            if(shouldUseModal)
                modal.gameObject.SetActive(false);
        }

        protected void Init()
        {
            if(!initted)
            {
                initted = true;

                startPos = windowRect.localPosition;
                startScale = windowRect.localScale;

                canvasGroup = GetComponent<CanvasGroup>();
                startAlpha = canvasGroup.alpha;

                if(shouldUseModal)
                {
                    // initializes the modal
                    modal = new GameObject("Modal").AddComponent<Image>();
                    m_rectTransform = GetComponent<RectTransform>();
                    RectTransform modalRect = modal.rectTransform;

                    // set modal color and store it's initial alpha value
                    modal.color = modalColor;
                    modalStartAlpha = modalColor.a;

                    // set modal as the first child of this object
                    modalRect.SetParent(m_rectTransform, false);
                    modalRect.SetAsFirstSibling();

                    // set the modal to stretch anchoring and stretch it to fit this object
                    modalRect.anchorMin = Vector2.zero;
                    modalRect.anchorMax = Vector2.one;
                    modalRect.pivot = new Vector2(0.5f, 0.5f);
                    modalRect.anchoredPosition = Vector2.zero;

                    Utils.Core.CoroutineRunner.CallAfterDelay(() => {
                        modalRect.SetWidth(m_rectTransform.rect.width);
                        modalRect.SetHeight(m_rectTransform.rect.height);
                    }, 0.1f, true);

                    // make the modal ignore the parent canvas group
                    modal.gameObject.AddComponent<CanvasGroup>().ignoreParentGroups = true;
                    
                    // add modal button component
                    Button modalButton = modal.gameObject.AddComponent<Button>();

                    // disable any color transtion on the modal button
                    modalButton.transition = Selectable.Transition.None;
                    // add modal click callback
                    modalButton.onClick.AddListener(ModalClick);
                }
                
                // add all buttons in the child to the buttons that should disable while animating
                Button[] m_buttons = GetComponentsInChildren<Button>();

                foreach(Button b in m_buttons)
                    if(!DisableWhileAnim.Contains(b))
                        DisableWhileAnim.Add(b);

                OnInit();
            }
        }

        private List<Vector2> GetOutSidePositions()
        {
            return new List<Vector2> {
                new Vector2(startPos.x,
                    (m_rectTransform.sizeDelta.y/2) +
                    (windowRect.sizeDelta.y/2) +
                    10),             // up
                new Vector2(startPos.x,
                    - (m_rectTransform.sizeDelta.y/2 +
                    windowRect.sizeDelta.y/2 +
                    10)),             // down
                new Vector2(
                    - (m_rectTransform.sizeDelta.x/2 +
                    windowRect.sizeDelta.x/2 +
                    10), startPos.y), // left
                new Vector2(
                    (m_rectTransform.sizeDelta.x/2) +
                    (windowRect.sizeDelta.x/2) +
                    10, startPos.y)  // right
            };
        }

        private void SetButtonsAnim(bool target)
        {
            foreach(Button b in DisableWhileAnim)
                if(b != null)
                    b.enabled = target;
        }

        private void SetButtonsWhileOpen(bool target)
        {
            foreach(Button b in DisableWhileOpen)
                if(b != null)
                    b.enabled = target;
        }
    }
}

