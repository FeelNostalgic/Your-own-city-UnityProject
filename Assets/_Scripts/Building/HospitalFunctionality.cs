using Managers;
using UnityEngine;

namespace Buildings
{
    public class HospitalFunctionality : MultiplierBuildingFunctionality
    {
        #region Unity Methods

        protected override void Start()
        {
            _buildingPrice = BuildManager.Instance.HospitalPrice;
            base.Start();
        }

        #endregion

        #region Protected Methods

        protected override void RegisterBuilding()
        {
            BuildingsManager.Instance.Hospitals.Add(this);
        }

        protected override void UnregisterBuilding()
        {
            BuildingsManager.Instance.Hospitals.Remove(this);
        }

        #region Build line renderer

        protected override Vector3[] BuildLineRenderer()
        {
            Vector3[] lineRendererPositions = null;
            switch (_effectArea)
            {
                case 1:
                    _neighbourTiles = MapManager.Instance.Get8Neighbours(GetParentGameObject());
                    lineRendererPositions = BuildLineRenderer8Neighbours();
                    break;
                case 2:
                    _neighbourTiles = MapManager.Instance.Get12Neightbour(GetParentGameObject());
                    lineRendererPositions = BuildLineRenderer12Neighbours();
                    break;
                case 3:
                    _neighbourTiles = MapManager.Instance.Get25Neighbour(GetParentGameObject());
                    lineRendererPositions = BuildLineRenderer25Neighbours();
                    break;
            }

            return lineRendererPositions;
        }

        private Vector3[] BuildLineRenderer25Neighbours()
        {
            _lineRenderer.positionCount = 36;
            var positions = new Vector3[_lineRenderer.positionCount];
            var linePositions = new Vector3[4];
            var i = 0;
            if (_neighbourTiles[0] != null)
            {
                _neighbourTiles[0].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_neighbourTiles[1] != null)
            {
                _neighbourTiles[1].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_neighbourTiles[2] != null)
            {
                _neighbourTiles[2].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_neighbourTiles[3] != null)
            {
                _neighbourTiles[3].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_neighbourTiles[4] != null)
            {
                _neighbourTiles[4].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[9] != null)
            {
                _neighbourTiles[9].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[14] != null)
            {
                _neighbourTiles[14].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[19] != null)
            {
                _neighbourTiles[19].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[24] != null)
            {
                _neighbourTiles[24].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[23] != null)
            {
                _neighbourTiles[23].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[22] != null)
            {
                _neighbourTiles[22].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[21] != null)
            {
                _neighbourTiles[21].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[20] != null)
            {
                _neighbourTiles[20].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_neighbourTiles[15] != null)
            {
                _neighbourTiles[15].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_neighbourTiles[10] != null)
            {
                _neighbourTiles[10].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_neighbourTiles[5] != null)
            {
                _neighbourTiles[5].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            return positions;
        }

        #endregion

        #endregion
    }
}