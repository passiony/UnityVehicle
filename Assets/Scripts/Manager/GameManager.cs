using System;
using UnityEngine;

namespace Vertical
{
    public class GameManager : MonoBehaviour
    {
        public bool DebugMode = true;
        public HUDLevel hudIndex;
        public SimulateLevel simulateIndex;
        public GameObject[] simulate1Go;
        public GameObject[] simulate2Go;

        private void Awake()
        {
            if (DebugMode)
            {
                GlobalData.simulateIndex = (int)simulateIndex;
                GlobalData.hudIndex = (int)hudIndex;
            }
        }

        private void Start()
        {
            if (GlobalData.simulateIndex == 0)
            {
                foreach (var go in simulate1Go)
                {
                    go.SetActive(true);
                }
            }
            else
            {
                foreach (var go in simulate2Go)
                {
                    go.SetActive(true);
                }
            }
        }
    }
}