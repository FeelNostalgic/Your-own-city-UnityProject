using System;
using UnityEngine;

namespace Residents
{
    [Serializable]
    public class HabitanteScriptable
    {
        #region Public Variables

        public string Name;
        public int BaseGold;
        public float MultiplicadorGold;
        public int GastosPorSegundo;
        public Gradient Color;

        #endregion
    }
}