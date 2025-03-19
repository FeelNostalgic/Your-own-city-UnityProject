using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class AnimateUI : MonoBehaviour
    {
        #region Inspector Variables

        [Range(0, 2f), SerializeField] private float duration = 0.3f;
        
        [Header("Position animation")] [SerializeField]
        private bool animatePosition;

        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Ease inEasePosition;
        [SerializeField] private Ease outEasePosition;

        [Header("Scale animation")] [SerializeField]
        private bool animateScale;

        [SerializeField] private Vector2 startScale = Vector2.one;
        [SerializeField] private Ease inEaseScale;
        [SerializeField] private Ease outEaseScale;

        #endregion

        #region Public Variables

        public float Duration => duration;
        public bool IsOpen { get; private set; }
        
        public event Action OnShowAnimationPlay;
        public event Action OnShowAnimationCompleted;
        public event Action OnHideAnimationPlay;
        public event Action OnHideAnimationCompleted;

        #endregion

        #region Private Variables

        private RectTransform _rectTransform;

        private Vector2 _endPosition;
        private Vector2 _endScale;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (animatePosition)
            {
                _endPosition = _rectTransform.anchoredPosition;
                _rectTransform.anchoredPosition = startPosition;
            }

            if (animateScale)
            {
                _endScale = _rectTransform.localScale;
                _rectTransform.localScale = startScale;
            }
        }

        #endregion

        #region Public Methods

        public void Show()
        {
            if (IsOpen) return;
            gameObject.SetActive(true);
            var sequence = DOTween.Sequence();

            if (animatePosition)
            {
                sequence.Append(_rectTransform.DOAnchorPos(_endPosition, duration)
                    .SetEase(inEasePosition));
            }

            if (animateScale)
            {
                sequence.Join(_rectTransform.DOScale(_endScale, duration)
                    .SetEase(inEaseScale));
            }

            sequence.OnPlay(() => OnShowAnimationPlay?.Invoke())
                .OnComplete(() =>
                {
                    IsOpen = true;
                    OnShowAnimationCompleted?.Invoke();
                });

            sequence.Play();
        }

        public void Hide()
        {
            if (!IsOpen) return;
            var sequence = DOTween.Sequence();
            
            if (animatePosition)
            {
                sequence.Append(_rectTransform.DOAnchorPos(startPosition, duration)
                    .SetEase(outEasePosition));
            }

            if (animateScale)
            {
                sequence.Join(_rectTransform.DOScale(startScale, duration)
                    .SetEase(outEasePosition));
            }

            sequence.OnPlay(() => OnHideAnimationPlay?.Invoke());
            sequence.OnComplete(() =>
            {
                IsOpen = false;
                gameObject.SetActive(true);
                OnHideAnimationCompleted?.Invoke();
            });

            sequence.Play();
        }

        #endregion
    }
}