    using UnityEngine;
    using System.Collections;
    using DG.Tweening;
    using UnityEngine.Events;

    public class UIAnimatedScale : MonoBehaviour
    {
        [Header("Animation Settings")] [SerializeField]
        private GameObject target;

        [SerializeField] private float duration = 0.25f;
        [SerializeField] private Vector3 startScale = Vector3.zero;
        [SerializeField] private Vector3 endScale = Vector3.one;
        [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] UnityEvent OnAnimationStart;
        [SerializeField] UnityEvent OnAnimationEnd;

        private RectTransform rectTransform;
        private Tween currentTween;
        private bool isAnimating;

        private void Awake()
        {
            isAnimating = false;
            if (target == null)
                target = gameObject;

            rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null)
                rectTransform = target.AddComponent<RectTransform>();

            // Set initial scale
            rectTransform.localScale = startScale;
        }

        private void OnEnable()
        {
            PlayScaleIn();
        }

        private void OnDisable()
        {
            if (currentTween != null)
                currentTween.Kill();

            rectTransform.localScale = startScale;
        }

        public void PlayScaleIn()
        {
            if (isAnimating) return;
            isAnimating = true;
            currentTween?.Kill();

            OnAnimationStart?.Invoke();

            rectTransform.localScale = startScale;

            currentTween = rectTransform
                .DOScale(endScale, duration)
                .SetEase(ease)
                .SetUpdate(true)
                .OnComplete(() => { isAnimating = false; });
        }

        public void HideWithAnimation()
        {
            if (isAnimating) return;
            isAnimating = true;
            
            currentTween?.Kill();

            currentTween = rectTransform
                .DOScale(startScale, duration)
                .SetEase(ease)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    isAnimating = false;
                    gameObject.SetActive(false);
                    OnAnimationEnd?.Invoke();
                });
        }
    }