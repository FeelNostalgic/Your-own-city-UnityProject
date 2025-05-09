using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using Managers;
using UnityEngine;

namespace Buildings
{
    public abstract class MultiplierBuildingFunctionality : Building
    {
        #region Inspector Variables

        [SerializeField] protected Light[] lights;

        [SerializeField] protected int startingCostsPerSecond;

        [SerializeField] protected float startingMultiplier;
        [SerializeField] protected float multiplierUpgradePerLevel;
        [SerializeField] private BuildingType type;
        
        #endregion

        #region Public Variables

        public int Level => _currentLevel;
        public int CostsPerSecond => _costsPerSecond;
        public float Multiplier => _currentMultiplier;
        public int EffectArea => _effectArea;
        public int AffectedHouses => _affectedHouses.Count;
        public int LevelPrice => _levelPrice;
        public float BuyPrice => _buildingPrice;
        public BuildingType BuildingType => type;
        public event Action OnLevelUpdate;

        #endregion

        #region Protected Variables

        protected int _currentLevel;
        protected int _costsPerSecond;
        protected float _currentMultiplier;
        protected int _effectArea;
        protected int _levelPrice;
        protected List<TileFunctionality> _affectedHouses;
        protected List<TileFunctionality> _neighbourTiles;
        protected LineRenderer _lineRenderer;
        protected float _buildingPrice;

        #endregion

        #region Unity Methods

        protected virtual void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            RegisterBuilding();
            _affectedHouses = new List<TileFunctionality>();
            _neighbourTiles = new List<TileFunctionality>();
            _currentLevel = 1;
            _costsPerSecond = startingCostsPerSecond;
            _currentMultiplier = 1 + startingMultiplier;
            _effectArea = 1;
            _levelPrice = (int)(_buildingPrice * 2f);
            ResourcesManager.AddCosts(_costsPerSecond);
            UpdateArea();
            UpdateMultiplierNewNeighbours();
            EnableLights();
        }

        #endregion

        #region Public Methods

        public void LevelUp()
        {
            if (ResourcesManager.CurrentGold > _levelPrice)
            {
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.levelUp);
                ResourcesManager.AddGold(-_levelPrice);
                _effectArea++;
                _currentLevel++;
                EnableLights();
                _levelPrice = (int)(_levelPrice * 2.2f);
                _currentMultiplier += multiplierUpgradePerLevel;
                ResourcesManager.AddCosts((int)(_costsPerSecond * 0.25f));
                _costsPerSecond = (int)(_costsPerSecond * 1.25f);
                UpdateArea();
                UpdateCurrentNeighboursMultiplier(multiplierUpgradePerLevel);
                UpdateMultiplierNewNeighbours();
                OnLevelUpdate?.Invoke();
                ShowArea();
            }
            else
            {
                UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
            }
        }

        public override void Demolish()
        {
            ResourcesManager.AddGold((int)(_buildingPrice * 0.8f * _currentLevel));
            ResourcesManager.AddCosts(-_costsPerSecond);
            UpdateCurrentNeighboursMultiplier(-(_currentMultiplier - 1));
            UnregisterBuilding();
            UIManagerInGame.Instance.DisableAllHUDExceptBuildPanel();
            Destroy(gameObject);
        }

        public void RemoveHouseFromAffectedHouses(TileFunctionality house)
        {
            if (_affectedHouses.Contains(house)) _affectedHouses.Remove(house);
        }

        public virtual void UpdateMultiplierNewNeighbours()
        {
            foreach (var tile in _neighbourTiles.Where(tile => tile != null && tile.BuildingType == BuildingType.house && !_affectedHouses.Contains(tile)))
            {
                _affectedHouses.Add(tile);
                tile.GetComponentInChildren<HouseFunctionality>().UpgradeMultiplier(multiplierUpgradePerLevel);
            }
        }

        public void UpdateArea()
        {
            var lineRendererPositions = BuildLineRenderer();
            _lineRenderer.SetPositions(lineRendererPositions);
            HideArea();
        }

        public void ShowArea()
        {
            _lineRenderer.enabled = true;
        }

        public void HideArea()
        {
            _lineRenderer.enabled = false;
        }

        #endregion

        #region Protected Methods

        protected abstract void RegisterBuilding();
        protected abstract void UnregisterBuilding();

        protected virtual void EnableLights()
        {
            if (lights is { Length: > 0 } && _currentLevel <= lights.Length)
            {
                lights[_currentLevel - 1].enabled = true;
            }
        }

        protected virtual Vector3 ParentPosition()
        {
            return transform.parent.position;
        }

        protected virtual void UpdateCurrentNeighboursMultiplier(float m)
        {
            foreach (var tile in _affectedHouses)
            {
                tile.GetComponentInChildren<HouseFunctionality>().UpgradeMultiplier(m);
            }
        }

        #region Build line renderer

        protected virtual Vector3[] BuildLineRenderer()
        {
            Vector3[] lineRendererPositions = null;
            switch (_effectArea)
            {
                case 1:
                    _neighbourTiles = MapManager.Instance.Get4Neighbours(ParentPosition());
                    lineRendererPositions = BuildLineRenderer4Neighbours();
                    break;
                case 2:
                    _neighbourTiles = MapManager.Instance.Get8Neighbours(ParentPosition());
                    lineRendererPositions = BuildLineRenderer8Neighbours();
                    break;
                case 3:
                    _neighbourTiles = MapManager.Instance.Get12Neighbours(ParentPosition());
                    lineRendererPositions = BuildLineRenderer12Neighbours();
                    break;
            }

            return lineRendererPositions;
        }

        protected Vector3[] BuildLineRenderer4Neighbours()
        {
            _lineRenderer.positionCount = 15;
            var positions = new Vector3[_lineRenderer.positionCount];
            var linePositions = new Vector3[4];
            var i = 0;
            if (_neighbourTiles[2] != null)
            {
                _neighbourTiles[2].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[0] != null)
            {
                _neighbourTiles[0].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_neighbourTiles[3] != null)
            {
                _neighbourTiles[3].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_neighbourTiles[1] != null)
            {
                _neighbourTiles[1].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            return positions;
        }

        protected Vector3[] BuildLineRenderer8Neighbours()
        {
            _lineRenderer.positionCount = 20;
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
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[5] != null)
            {
                _neighbourTiles[5].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[8] != null)
            {
                _neighbourTiles[8].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[7] != null)
            {
                _neighbourTiles[7].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[6] != null)
            {
                _neighbourTiles[6].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_neighbourTiles[3] != null)
            {
                _neighbourTiles[3].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            return positions;
        }

        protected Vector3[] BuildLineRenderer12Neighbours()
        {
            _lineRenderer.positionCount = 28;
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

            if (_neighbourTiles[10] != null)
            {
                _neighbourTiles[10].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[2] != null)
            {
                _neighbourTiles[2].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_neighbourTiles[11] != null)
            {
                _neighbourTiles[11].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[8] != null)
            {
                _neighbourTiles[8].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_neighbourTiles[9] != null)
            {
                _neighbourTiles[9].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_neighbourTiles[6] != null)
            {
                _neighbourTiles[6].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_neighbourTiles[12] != null)
            {
                _neighbourTiles[12].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            return positions;
        }

        #endregion

        #endregion
    }
}