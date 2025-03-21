using System;
using Commons;
using Managers;
using UnityEngine;

namespace Buildings
{ 
	public class TileFunctionality : MonoBehaviour, IHighlight
	{
		#region Inspector Variables
		
		[SerializeField] private Material lineRendererDemolishMaterial;
		
		#endregion
	
		#region Public Variables

		public BuildingType BuildingType { get; set; }
		public HighlightType HighlightState { get; private set; }
		public Vector2Int MapPosition { get; set; }
		public Building Building { get; set; }
			
		#endregion

		#region Private Variables
		
		private LineRenderer _lineRenderer;
		private Material _defaultLineRendererMaterial;
		
		#endregion

		#region Unity Methods
		
		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			_defaultLineRendererMaterial = _lineRenderer.material;
		}
		
		#endregion

		#region Public Methods
		
		public void Highlight()
		{
			((IHighlight)this).ChangeColor(BuildManager.Status);
			_lineRenderer.enabled = true;
			HighlightState = HighlightType.highlighted;
		}

		public void UpdateColor()
		{
			((IHighlight)this).ChangeColor(BuildManager.Status);
		}

		public void Unhighlight()
		{
			if (HighlightState == HighlightType.selected) return;
			_lineRenderer.enabled = false;
			HighlightState = HighlightType.none;
		}

		public void Select()
		{
			_lineRenderer.enabled = true;
			HighlightState = HighlightType.selected;
		}

		public void Deselect()
		{
			_lineRenderer.enabled = false;
			HighlightState = HighlightType.none;
		}


		#endregion

		#region Private Methods
		
		void IHighlight.ChangeColor(BuildingStatus buildingStatus)
		{
			_lineRenderer.material = buildingStatus switch
			{
				BuildingStatus.none => _defaultLineRendererMaterial,
				BuildingStatus.demolishing => lineRendererDemolishMaterial,
				BuildingStatus.building => _defaultLineRendererMaterial,
				_ => throw new ArgumentOutOfRangeException(nameof(buildingStatus), buildingStatus, null)
			};
		}
		
		#endregion
	}

	public interface IHighlight
	{
		public void Highlight();
		public void UpdateColor();

		public void Unhighlight();
		public void Select();

		public void Deselect();

		protected internal void ChangeColor(BuildingStatus buildingStatus);
	}
}