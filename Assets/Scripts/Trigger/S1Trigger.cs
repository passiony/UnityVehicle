using System;
using System.Collections;
using GleyTrafficSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Vertical
{
    public class S1Trigger : MonoBehaviour
    {
        public AutoCar m_DecCar;
        public PlayerCar m_PlayerCar;
        private HUDManager m_HUDManager;
        public AutoCar[] m_SideCars;

        private void Start()
        {
            m_DecCar.gameObject.SetActive(true);
            m_HUDManager = gameObject.GetComponent<HUDManager>();
            int active = Random.Range(0, 3);
            if (active < 2)
            {
                m_SideCars[active].gameObject.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var body = other.GetComponentInParent<Rigidbody>();
            if (body && body.CompareTag("Player"))
            {
                m_DecCar.StartTrigger();
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