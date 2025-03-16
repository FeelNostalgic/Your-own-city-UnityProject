using System.Collections.Generic;
using Managers;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Buildings
{
    public class ParqueFunctionality : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private int GastorPorSegundoIniciales;
        [SerializeField] private float MultiplicadorInicial;

        #endregion

        #region Public Variables

        public int Level => _currentLevel;
        public int GastosPorSegundo => _gastosPorSegundo;
        public float Multiplicador => _currentMultiplicador;
        public int AreaEfecto => _areaEfecto;
        public int CasaAfectadas => _casasAfectadas.Count;
        public int CosteNivel => _costeNivel;

        #endregion

        #region Private Variables

        private int _currentLevel;
        private int _gastosPorSegundo;
        private float _currentMultiplicador;
        private int _areaEfecto;
        private int _costeNivel;
        private List<GameObject> _casasAfectadas;
        private List<GameObject> _tilesVecinas;
        private const float MULTIPLIER = 0.25f;
        private LineRenderer _lineRenderer;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            BuildingsManager.Instance.Parques.Add(this);
            _casasAfectadas = new List<GameObject>();
            _tilesVecinas = new List<GameObject>();
            _currentLevel = 1;
            _gastosPorSegundo = GastorPorSegundoIniciales;
            _currentMultiplicador = 1 + MultiplicadorInicial;
            _areaEfecto = 1;
            _costeNivel = (int)(BuildManager.Instance.ParquePrice * 2f);
            ResourcesManager.Instance.AddCosts(_gastosPorSegundo);
            UpdateArea();
            UpdateMultiplicadorNewVecinos();
        }

        #endregion

        #region Public Methods

        public void SubirNivel()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold > _costeNivel)
            {
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.levelUp);
                ResourcesManager.Instance.AddGold(-_costeNivel);
                _areaEfecto++;
                _currentLevel++;
                _costeNivel = (int)(_costeNivel * 2.2f);
                _currentMultiplicador += MULTIPLIER;
                ResourcesManager.Instance.AddCosts((int)(_gastosPorSegundo * 0.25f));
                _gastosPorSegundo = (int)(_gastosPorSegundo * 1.25f);
                UpdateArea();
                UpdateMultiplicadorCurrentVecinos(MULTIPLIER);
                UpdateMultiplicadorNewVecinos();
                UIManagerInGame.Instance.UpdateInfoMultiplierNivel(_currentLevel, _gastosPorSegundo, _currentMultiplicador,
                    _areaEfecto, _costeNivel, _casasAfectadas.Count, BuildManager.Instance.ParquePrice);
                ShowArea();
            }
            else
            {
                UIManagerInGame.Instance.UpdateFeedback("¡Gold insuficiente!");
            }
        }

        public void Demoler()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.detroyBuilding);
            ResourcesManager.Instance.AddGold((int)(BuildManager.Instance.ParquePrice * 0.8f * _currentLevel));
            ResourcesManager.Instance.AddCosts(-_gastosPorSegundo);
            UpdateMultiplicadorCurrentVecinos(-(_currentMultiplicador - 1));
            GetComponentInParent<BuildType>().Type = BuildManager.BuildingType.none;
            BuildingsManager.Instance.Parques.Remove(this);
            UIManagerInGame.Instance.DisableAllPanels();
            Destroy(gameObject);
        }

        public void RemoveCasa(GameObject casa)
        {
            if (_casasAfectadas.Contains(casa)) _casasAfectadas.Remove(casa);
        }

        public void UpdateMultiplicadorNewVecinos()
        {
            foreach (var v in _tilesVecinas)
            {
                if (v != null && v.GetComponent<BuildType>().Type == BuildManager.BuildingType.casa &&
                    !_casasAfectadas.Contains(v))
                {
                    _casasAfectadas.Add(v);
                    v.GetComponentInChildren<CasaFunctionality>().MejorarMultiplicador(_currentMultiplicador - 1);
                }
            }
        }

        public void UpdateArea()
        {
            Vector3[] lineRendererPositions = null;
            switch (_areaEfecto)
            {
                case 1:
                    _tilesVecinas =
                        MapManager.Instance.GetVecinos4(gameObject); //0 -> derecha, 1-> Izquierda, 2-> Up, 3-> down
                    lineRendererPositions = BuildLineRenderer4();
                    break;
                case 2:
                    _tilesVecinas = MapManager.Instance.GetVecinos8(gameObject);
                    lineRendererPositions = BuildLineRenderer8();
                    break;
                case 3:
                    _tilesVecinas = MapManager.Instance.GetVecinos12(gameObject);
                    lineRendererPositions = BuildLineRenderer12();
                    break;
            }

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

        #region Private Methods

        private void UpdateMultiplicadorCurrentVecinos(float m)
        {
            foreach (var c in _casasAfectadas)
            {
                c.GetComponentInChildren<CasaFunctionality>().MejorarMultiplicador(m);
            }
        }

        private Vector3[] BuildLineRenderer4()
        {
            _lineRenderer.positionCount = 15;
            Vector3[] positions = new Vector3[_lineRenderer.positionCount];
            var linePositions = new Vector3[4];
            int i = 0;
            if (_tilesVecinas[2] != null)
            {
                _tilesVecinas[2].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_tilesVecinas[0] != null)
            {
                _tilesVecinas[0].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_tilesVecinas[3] != null)
            {
                _tilesVecinas[3].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_tilesVecinas[1] != null)
            {
                _tilesVecinas[1].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            return positions;
        }

        private Vector3[] BuildLineRenderer8()
        {
            _lineRenderer.positionCount = 20;
            Vector3[] positions = new Vector3[_lineRenderer.positionCount];
            var linePositions = new Vector3[4];
            int i = 0;
            if (_tilesVecinas[0] != null)
            {
                _tilesVecinas[0].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_tilesVecinas[1] != null)
            {
                _tilesVecinas[1].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_tilesVecinas[2] != null)
            {
                _tilesVecinas[2].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_tilesVecinas[5] != null)
            {
                _tilesVecinas[5].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_tilesVecinas[8] != null)
            {
                _tilesVecinas[8].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_tilesVecinas[7] != null)
            {
                _tilesVecinas[7].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_tilesVecinas[6] != null)
            {
                _tilesVecinas[6].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_tilesVecinas[3] != null)
            {
                _tilesVecinas[3].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            return positions;
        }

        private Vector3[] BuildLineRenderer12()
        {
            _lineRenderer.positionCount = 28;
            Vector3[] positions = new Vector3[_lineRenderer.positionCount];
            var linePositions = new Vector3[4];
            int i = 0;
            if (_tilesVecinas[0] != null)
            {
                _tilesVecinas[0].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            if (_tilesVecinas[10] != null)
            {
                _tilesVecinas[10].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_tilesVecinas[2] != null)
            {
                _tilesVecinas[2].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
            }

            if (_tilesVecinas[11] != null)
            {
                _tilesVecinas[11].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_tilesVecinas[8] != null)
            {
                _tilesVecinas[8].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
            }

            if (_tilesVecinas[9] != null)
            {
                _tilesVecinas[9].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[1];
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_tilesVecinas[6] != null)
            {
                _tilesVecinas[6].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
            }

            if (_tilesVecinas[12] != null)
            {
                _tilesVecinas[12].GetComponent<LineRenderer>().GetPositions(linePositions);
                positions[i++] = linePositions[2];
                positions[i++] = linePositions[3];
                positions[i++] = linePositions[0];
                positions[i++] = linePositions[1];
            }

            return positions;
        }

        #endregion
    }
}