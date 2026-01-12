using System;
using System.Collections;
using GleyTrafficSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Vertical
{
    public class S2Trigger : MonoBehaviour
    {
        public ChildDartOut[] m_Childs;
        public PlayerCar m_PlayerCar;
        private HUDManager m_HUDManager;
        public FollowChild m_FollowChild;
        public SafetyArea[] m_SafetyArea;

        private ChildDartOut m_TargetChild;

        void Start()
        {
            m_HUDManager = gameObject.GetComponent<HUDManager>();
            var random = Random.Range(0, m_Childs.Length);
            m_TargetChild = m_Childs[random];
            m_TargetChild.gameObject.SetActive(true);

            m_FollowChild.SetTarget(m_TargetChild.transform, random);
            foreach (var safe in m_SafetyArea)
            {
                safe.SetTarget(m_TargetChild.transform);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var body = other.GetComponentInParent<Rigidbody>();
            if (body && body.CompareTag("Player"))
            {
                m_TargetChild.StartTrigger();
                m_PlayerCar.GetComponent<AutoCar>().enabled = false;
                m_PlayerCar.Drivable = true;
                m_HUDManager.ShowTakeoverRequest();

                DataCenter.Instance.TriggerRiskEvent();
                StartCoroutine(DelayToLaunch());
            }
        }

        IEnumerator DelayToLaunch()
        {
            yield return new WaitForSeconds(10f);
            SceneManager.LoadScene("Launch");
        }
    }
}