
using System;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{ 
	public class GameManager : MonoBehaviourSinglenton<GameManager>
	{
		#region Public Variables
		
		public static event Action<GameState> OnBeforeStateChange;
		public static event Action<GameState> OnAfterStateChange;
		
		public static GameState CurrentGameState { get; private set; }
		
		#endregion

		#region Private Variables
		//
		#endregion

		#region Unity Methods

		private void Start() => ChangeState(GameState.NotStarted);
		
		#endregion

		#region Public Methods

		public void ChangeState(GameState newGameState)
		{
			OnBeforeStateChange?.Invoke(newGameState);

			CurrentGameState = newGameState;
			switch (newGameState)
			{
				case GameState.NotStarted:
					break;
				case GameState.Starting:
					break;
				case GameState.Paused:
					break;
				case GameState.Playing:
					break;
				case GameState.GameOver:
					break;
				case GameState.Win:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
			}
			
			OnAfterStateChange?.Invoke(newGameState);
		}
		
		#endregion

		#region Private Methods
		//
		#endregion
	}

	[Serializable]
	public enum GameState
	{
		NotStarted, Starting, Paused, Playing, GameOver, Win
	}
}