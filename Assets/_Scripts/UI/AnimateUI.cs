using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class AnimateUI : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Position animation")] [SerializeField]
        private bool animatePosition;

        [SerializeField] private Vector2 startPosition;
        [Range(0, 2f), SerializeField] private float positionDuration;
        [SerializeField] private Ease inEasePosition;
        [SerializeField] private Ease outEasePosition;

        [Header("Scale animation")] [SerializeField]
        private bool animateScale;

        [SerializeField] private Vector2 startScale = Vector2.one;
        [Range(0, 2f), SerializeField] private float scaleDuration;
        [SerializeField] private Ease inEaseScale;
        [SerializeField] private Ease outEaseScale;

        #endregion

        #region Public Variables

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
            gameObject.SetActive(true);
            var sequence = DOTween.Sequence();

            if (animatePosition)
            {
                sequence.Append(_rectTransform.DOAnchorPos(_endPosition, positionDuration)
                    .SetEase(inEasePosition));
            }

            if (animateScale)
            {
                sequence.Join(_rectTransform.DOScale(_endScale, scaleDuration)
                    .SetEase(inEaseScale));
            }

            sequence.OnPlay(() => OnShowAnimationPlay?.Invoke())
                .OnComplete(() => OnShowAnimationCompleted?.Invoke());

            sequence.Play();
        }

        public void Hide()
        {
            var sequence = DOTween.Sequence();


            if (animatePosition)
            {
                sequence.Append(_rectTransform.DOAnchorPos(startPosition, positionDuration)
                    .SetEase(outEasePosition));
            }

            if (animateScale)
            {
                sequence.Join(_rectTransform.DOScale(startScale, scaleDuration)
                    .SetEase(outEasePosition));
            }

            sequence.OnPlay(() => OnHideAnimationPlay?.Invoke());
            sequence.OnComplete(() =>
            {
                gameObject.SetActive(true);
                OnHideAnimationCompleted?.Invoke();
            });

            sequence.Play();
        }

        #endregion
    }
}