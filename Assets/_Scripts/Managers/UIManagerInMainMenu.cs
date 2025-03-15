
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{ 
	public class UIManagerInMainMenu : MonoBehaviour
	{
		#region Inspector Variables
		
		[Header("StartMenu")] 
		[SerializeField] private GameObject StartPanel;
		[SerializeField] private GameObject StartButton;
		
		[Header("Transition")]
		[SerializeField] private Image blackScreen;
		
		#endregion
	
		#region Public Variables
		
		#endregion

		#region Private Variables
		
		#endregion

		#region Unity Methods
		
		#endregion

		#region Public Methods
		
		public void StartGame()
		{
			AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
			blackScreen.DOFade(1f, 1.5f).SetEase(Ease.OutSine)
				.OnComplete(() =>
				{
					MySceneManager.LoadScene(MySceneManager.Scenes.GameScene);
				})
				.Play();
		}
		
		#endregion

		#region Private Methods
		
		#endregion
	}
}