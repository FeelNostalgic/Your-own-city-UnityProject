using System;
using Managers;
using UnityEngine;

namespace Buildings
{
    public class LineRendererHighlight : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private Material lineRendererDemolishMaterial;

        #endregion

        #region Public Variables

        public HighlightType HighlightState { get; private set; }

        public enum HighlightType
        {
            none,
            highlighted,
            selected
        }
        
        #endregion

        #region Private Variables

        private LineRenderer _lineRenderer;
        private Material _defaultMaterial;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _defaultMaterial = _lineRenderer.material;
        }

        #endregion

        #region Public Methods

        public void Highlight()
        {
            ChangeColor(BuildManager.Status);
            _lineRenderer.enabled = true;
            HighlightState = HighlightType.highlighted;
        }

        public void UpdateColor()
        {
            ChangeColor(BuildManager.Status);
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

        private void ChangeColor(BuildManager.BuildingStatus buildingStatus)
        {
            _lineRenderer.material = buildingStatus switch
            {
                BuildManager.BuildingStatus.none => _defaultMaterial,
                BuildManager.BuildingStatus.demolishing => lineRendererDemolishMaterial,
                BuildManager.BuildingStatus.building => _defaultMaterial,
                _ => throw new ArgumentOutOfRangeException(nameof(buildingStatus), buildingStatus, null)
            };
        }

        #endregion
    }
}