using System.Collections;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI
{
    public class GeneralInfoController : MonoBehaviour
    {
        #region Inspector Variables

        //[SerializeField] private TMP_Text timeTMP;
        [SerializeField] private TMP_Text goldTMP;
        [SerializeField] private TMP_Text inhabitantsTMP;
        [SerializeField] private TMP_Text costsPerSecondTMP;
        [SerializeField] private TMP_Text goldPerSecondTMP;
        [SerializeField] private AnimateUI feedbackPanel;
        [SerializeField] private TMP_Text feedbackTMP;
        [Range(0, 3f), SerializeField] private float feedbackActiveTimeInSeconds;

        #endregion

        #region Public Variables

        //

        #endregion

        #region Private Variables

        private Sequence _feedbackSequence;
        private Coroutine _feedbackCoroutine;
        private bool _isFeedbackActive;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            ResourcesManager.Instance.OnGoldUpdate += UpdateTotalGoldTMP;
            ResourcesManager.Instance.OnResidentUpdate += UpdateInhabitantsNumberTMP;
            ResourcesManager.Instance.OnCostsPerSecondUpdate += UpdateCostsPerSecondTMP;
            ResourcesManager.Instance.OnGoldPerSecondUpdate += UpdateGoldPerSecondTMP;
        }

        private void OnDestroy()
        {
            ResourcesManager.Instance.OnGoldUpdate -= UpdateTotalGoldTMP;
            ResourcesManager.Instance.OnResidentUpdate -= UpdateInhabitantsNumberTMP;
            ResourcesManager.Instance.OnCostsPerSecondUpdate -= UpdateCostsPerSecondTMP;
            ResourcesManager.Instance.OnGoldPerSecondUpdate -= UpdateGoldPerSecondTMP;
        }

        #endregion

        #region Public Methods

        public void ShowFeedback(string info)
        {
            if (_isFeedbackActive.Not()) _feedbackCoroutine = StartCoroutine(ShowFeedbackCoroutine(info));
            else
            {
                StopAllCoroutines();
                _feedbackCoroutine = StartCoroutine(UpdateFeedbackCoroutine(info));
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator ShowFeedbackCoroutine(string info)
        {
            _isFeedbackActive = true;
            feedbackTMP.text = info;
            feedbackPanel.Show();
            yield return new WaitForSeconds(feedbackActiveTimeInSeconds);
            feedbackPanel.Hide();
            _isFeedbackActive = false;
        }

        private IEnumerator UpdateFeedbackCoroutine(string info)
        {
            _isFeedbackActive = true;
            _feedbackSequence = DOTween.Sequence();
            _feedbackSequence.Append(feedbackTMP.rectTransform.DOScale(Vector3.zero, 0.3f)
                .OnComplete(() => { feedbackTMP.text = info; }));
            _feedbackSequence.AppendInterval(0.1f);
            _feedbackSequence.Append(feedbackTMP.rectTransform.DOScale(Vector3.one, 0.3f));
            _feedbackSequence.Play();
            yield return new WaitForSeconds(feedbackActiveTimeInSeconds);
            feedbackPanel.Hide();
            _isFeedbackActive = false;
        }

        private void UpdateTotalGoldTMP(int g)
        {
            goldTMP.text = $"{g:N0}";
            goldTMP.color = g < 0 ? Color.red : Color.white;
        }

        private void UpdateInhabitantsNumberTMP(int h)
        {
            inhabitantsTMP.text = h.ToString();
        }

        private void UpdateGoldPerSecondTMP(int g)
        {
            goldPerSecondTMP.text = g.ToString(); //string.Format("{0:N0}", g);
        }

        private void UpdateCostsPerSecondTMP(int g)
        {
            costsPerSecondTMP.text = g.ToString(); //string.Format("{0:N0}", g);
        }

        // private void UpdateTime()
        // {
        // 	_currentTime += Time.deltaTime;
        // 	var minutes = Mathf.FloorToInt(_currentTime / 60);
        // 	var seconds = Mathf.FloorToInt(_currentTime % 60);
        // 	timeTMP.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        // }

        #endregion
    }
}