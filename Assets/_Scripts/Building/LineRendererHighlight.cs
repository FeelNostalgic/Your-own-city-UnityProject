using UnityEngine;

namespace Buildings
{ 
	public class LineRendererHighlight : MonoBehaviour
	{
		#region Inspector Variables
		
		#endregion
	
		#region Public Variables
		
		public HighlightType HighlightState { get; private set; }
		
		public enum HighlightType
		{
			none, highlighted, selected
		}
		
		#endregion

		#region Private Variables
		
		private LineRenderer _lineRenderer;
		
		#endregion

		#region Unity Methods

		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
		}

		#endregion

		#region Public Methods

		public void Highlight()
		{
			_lineRenderer.enabled = true;
			HighlightState = HighlightType.highlighted;
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
		
		#endregion
	}
}