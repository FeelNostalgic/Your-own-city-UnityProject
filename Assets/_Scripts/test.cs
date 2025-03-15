using System;
using TMPro;
using UnityEngine;

namespace test
{
    public class test : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private TMP_Text textprueba;

        #endregion

        #region Public Variables

        #endregion

        #region Private Variables

        private int i = -1;
        private float _timer;
        #endregion

        #region Unity Methods

        private void Start()
        {
            // Application.targetFrameRate = 60;
            // QualitySettings.vSyncCount = 1;
        }

        private void Update()
        {
            if ((Time.frameCount % Application.targetFrameRate) == 0)
            {
                _timer = 0;
                textprueba.text = (i++.ToString());
            }
            else _timer += Time.deltaTime;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}