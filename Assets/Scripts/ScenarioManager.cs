using System;
using UnityEngine;

namespace DefaultNamespace
{
    public enum ScenarioType
    {
        RearEnd, //纵向追尾
        DartOut //横向鬼探头
    }
    
    public class ScenarioManager : MonoBehaviour
    {
        public static ScenarioManager Instance;

        public ScenarioType type;

        private void Awake()
        {
            Instance = this;
        }

        //开始事故剧情
        public void StartScenario()
        {
            switch (type)
            {
                case ScenarioType.RearEnd:
                    // 1.                             
                    break;
                case ScenarioType.DartOut:
                    // 2.                             
                    break;
            }
        }
    }
}