using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ScenarioManager : MonoBehaviour
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
                GameData.simulateIndex = (int)simulateIndex;
                GameData.hudIndex = (int)hudIndex;
            }
        }

        private void Start()
        {
            if (GameData.simulateIndex == 0)
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