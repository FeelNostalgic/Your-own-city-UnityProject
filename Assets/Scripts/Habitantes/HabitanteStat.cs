using Proyecto.Habitantes.Scriptable;
using UnityEngine;

namespace Proyecto.Habitantes
{
    public class HabitanteStat : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private HabitanteScriptableObject habitanteScriptableStats;

        #endregion

        #region Public Variables

        public int BaseGold => habitanteScriptableStats.habitanteScriptable.BaseGold;
        public float MultiplicadorGold => habitanteScriptableStats.habitanteScriptable.MultiplicadorGold;
        public int GastosPorSegundo => habitanteScriptableStats.habitanteScriptable.GastosPorSegundo;
        public Color EmissionColor => _emissionColor;
        
        #endregion

        #region Private Variables

        private Gradient _colorGradient;
        private Color _emissionColor;
        #endregion
        
        #region Unity Methods

        private void Start()
        {
            _colorGradient = habitanteScriptableStats.habitanteScriptable.Color;
            _emissionColor = _colorGradient.Evaluate(Random.value);
            GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", _emissionColor);
        }

        #endregion

    }
}