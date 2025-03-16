
using System;
using System.Collections;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
		[SerializeField] private TMP_Text feedbackTMP;
		[Range(0,3f), SerializeField] private float feedbackActiveTimeInSeconds;
		
		#endregion
	
		#region Public Variables
		//
		#endregion

		#region Private Variables
		
		private Sequence _feedbackTween;
		
		#endregion

		#region Unity Methods

		private void Awake()
		{
			feedbackTMP.SetAlpha(0);
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
			if (_feedbackTween != null && _feedbackTween.IsPlaying())
			{
				_feedbackTween.Kill();
				_feedbackTween = DOTween.Sequence();
				_feedbackTween.Append(feedbackTMP.DOFade(0f, 0.3f).SetEase(Ease.OutSine));
				_feedbackTween.Append(feedbackTMP
					.DOFade(1f, 0.5f).SetEase(Ease.InSine)
					.OnPlay(() =>
					{
						feedbackTMP.text = info;
					}));
				_feedbackTween.AppendInterval(feedbackActiveTimeInSeconds);
				_feedbackTween.Append(feedbackTMP.DOFade(0f, 0.5f).SetEase(Ease.OutSine));
				_feedbackTween.Play();
			}
			else
			{
				_feedbackTween = DOTween.Sequence();
				_feedbackTween.Append(feedbackTMP
					.DOFade(1f, 0.5f).SetEase(Ease.InSine)
					.OnPlay(() =>
					{
						feedbackTMP.text = info;
					}));
				_feedbackTween.AppendInterval(feedbackActiveTimeInSeconds);
				_feedbackTween.Append(feedbackTMP.DOFade(0f, 0.5f).SetEase(Ease.OutSine));
				_feedbackTween.Play();
			}
		}
		
		#endregion

		#region Private Methods
		
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