using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Vertical
{
    public class Launch : MonoBehaviour
    {
        public TMP_Dropdown hudDropdown;
        public TMP_Dropdown simulateDropdown;

        public Button startBtn;

        void Start()
        {
            startBtn.onClick.AddListener(OnStartClick);
        }

        private void OnStartClick()
        {
            var hudIndex = hudDropdown.value;
            var simulateIndex = simulateDropdown.value;
            GlobalData.hudIndex = hudIndex;
            GlobalData.simulateIndex = simulateIndex;

            SceneManager.LoadScene("SampleScene");
        }
    }
}