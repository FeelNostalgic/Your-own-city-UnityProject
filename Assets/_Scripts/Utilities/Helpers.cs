
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

namespace Utilities
{ 
	public static class Helpers
	{
		private static Camera _camera;

		public static Camera Camera
		{
			get
			{
				if(_camera == null) _camera = Camera.main;
				return _camera;
			}
		}

		private static readonly Dictionary<float, WaitForSeconds> WaitForSeconds = new();
		/// <summary>
		/// Use when you want to allocate memory every time you create a new WaitForSeconds
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static WaitForSeconds GetWaitForSecondNonAllocate(float time)
		{
			if(WaitForSeconds.TryGetValue(time, out var wait)) return wait;
			
			WaitForSeconds[time] = new WaitForSeconds(time);
			return WaitForSeconds[time];
		}
		
		private static PointerEventData _eventDataCurrentPosition;
		private static List<RaycastResult> _results;

		public static bool IsOverUI()
		{
			_eventDataCurrentPosition = new PointerEventData(EventSystem.current){position = Input.mousePosition};
			_results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
			return _results.Count > 0;
		}

		public static void DestroyChildren(Transform parent)
		{
			foreach (var child in parent.GetComponentsInChildren<GameObject>())
			{
				Object.Destroy(child);
			}
		}
		
		public static void DestroyImmediateChildren(Transform parent)
		{
			foreach (var child in parent.GetComponentsInChildren<GameObject>())
			{
				Object.DestroyImmediate(child);
			}
		}

		public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
		{
			RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
			return result;
		}

		public static string GetLocalizedString(string tableName, string key)
		{
			var table = LocalizationSettings.StringDatabase.GetTable(tableName);
			if (table != null)
			{
				var entry = table.GetEntry(key);
				if (entry != null)
				{
					return entry.GetLocalizedString();
				}
				else
				{
					Debug.LogError($"Key '{key}' not found in table '{tableName}'");
				}
			}
			else
			{
				Debug.LogError($"Table '{tableName}' not found");
			}
			return string.Empty;
		}
		
	}
}