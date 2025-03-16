using DG.Tweening;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class AnimateUI : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Position animation")]
        [SerializeField] private bool animatePosition;
        [SerializeField] private Vector2 startPosition;
        [Range(0, 2f),SerializeField] private float positionDuration;
        [SerializeField] private Ease inEasePosition;
        [SerializeField] private Ease outEasePosition;

        [Header("Scale animation")] 
        [SerializeField] private bool animateScale;
        [SerializeField] private Vector2 startScale = Vector2.one;
        [SerializeField] private float scaleDuration;
        [SerializeField] private Ease inEaseScale;
        [SerializeField] private Ease outEaseScale;
        
        
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
            _endPosition = _rectTransform.anchoredPosition;
            _endScale = _rectTransform.localScale;
            _rectTransform.anchoredPosition = startPosition;
            _rectTransform.localScale = startScale;
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
            
            sequence.OnComplete(()=>gameObject.SetActive(true));
            sequence.Play();
        }

        #endregion
    }
}