using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Utils
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

        public enum AnimationEasings
        {
            Linear = Ease.Linear,
            Smooth = Ease.InOutSine,
            OverSmooth = Ease.InOutCubic,
            Elastic = Ease.OutBack
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
        [SerializeField] protected AnimationEasings openCurve;

        [Tooltip("The duration of the opening animation.")]
        [SerializeField] protected float openDuration = 1;

        [Tooltip("The type of animation used when closing the window.")]
        [SerializeField] protected AnimationType closeAnimation;
        
        [Tooltip("The animation easing that the closing animation should use.")]
        [SerializeField] protected AnimationEasings closeCurve;

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
                
                windowRect.DOKill();

                if(shouldUseModal)
                    modal.DOKill();

                canvasGroup.DOKill();
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
                
                windowRect.DOKill();

                if(shouldUseModal)
                    modal.DOKill();

                canvasGroup.DOKill();
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
                    windowRect.DOScale(startScale, openDuration).SetEase((Ease) openCurve).SetUpdate(true);
                    break;

                case AnimationType.Fade:
                    canvasGroup.alpha = 0;
                    yield return null;
                    canvasGroup.DOFade(startAlpha, openDuration).SetEase((Ease) openCurve).SetUpdate(true);
                    break;

                case AnimationType.SlideUp:
                    windowRect.localPosition = outSidePositions[0];
                    yield return null;
                    windowRect.DOLocalMove(startPos, openDuration)
                              .SetEase((Ease) openCurve)
                              .SetUpdate(true);
                    break;

                case AnimationType.SlideDown:
                    windowRect.localPosition = outSidePositions[1];
                    yield return null;
                    windowRect.DOLocalMove(startPos, openDuration)
                              .SetEase((Ease) openCurve)
                              .SetUpdate(true);
                    break;

                case AnimationType.SlideLeft:
                    windowRect.localPosition = outSidePositions[2];
                    yield return null;
                    windowRect.DOLocalMove(startPos, openDuration)
                              .SetEase((Ease) openCurve)
                              .SetUpdate(true);
                    break;

                case AnimationType.SlideRight:
                    windowRect.localPosition = outSidePositions[3];
                    yield return null;
                    windowRect.DOLocalMove(startPos, openDuration)
                              .SetEase((Ease) openCurve)
                              .SetUpdate(true);
                    break;
            }

            // Start modal fade animation
            if(shouldUseModal)
                modal.DOFade(modalStartAlpha, openDuration).SetUpdate(true);

            // Disable buttons that should be disabled while the animation is playing
            SetButtonsAnim(false);

            yield return new WaitForSecondsRealtime(openDuration);
            
            // Enable buttons that should be disabled once animation stops playing
            SetButtonsAnim(true);

            currentCoroutine = null;
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
                    windowRect.DOScale(Vector3.zero, closeDuration).SetEase((Ease) closeCurve).SetUpdate(true);
                    break;

                case AnimationType.Fade:
                    canvasGroup.DOFade(0, closeDuration).SetEase((Ease) closeCurve).SetUpdate(true);
                    break;

                case AnimationType.SlideUp:
                    windowRect.DOLocalMove(outSidePositions[0], closeDuration)
                              .SetEase((Ease) closeCurve)
                              .SetUpdate(true);
                    break;

                case AnimationType.SlideDown:
                    windowRect.DOLocalMove(outSidePositions[1], closeDuration)
                              .SetEase((Ease) closeCurve)
                              .SetUpdate(true);
                    break;

                case AnimationType.SlideLeft:
                    windowRect.DOLocalMove(outSidePositions[2], closeDuration)
                              .SetEase((Ease) closeCurve)
                              .SetUpdate(true);
                    break;

                case AnimationType.SlideRight:
                    windowRect.DOLocalMove(outSidePositions[3], closeDuration)
                              .SetEase((Ease) closeCurve)
                              .SetUpdate(true);
                    break;
            }

            // Start modal fade animation
            if(shouldUseModal)
                modal.DOFade(0, closeDuration).SetUpdate(true);

            // Disable buttons that should be disabled while the animation is playing
            SetButtonsAnim(false);

            yield return new WaitForSecondsRealtime(closeDuration);

            // Enable buttons that should be disabled once animation stops playing
            SetButtonsAnim(true);

            currentCoroutine = null;
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
                    modalRect.anchorMin = new Vector2(0, 0);
                    modalRect.anchorMax = new Vector2(1, 1);
                    modalRect.pivot = new Vector2(0.5f, 0.5f);
                    modalRect.SetWidth(m_rectTransform.sizeDelta.x);
                    modalRect.SetHeight(m_rectTransform.sizeDelta.y);

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

