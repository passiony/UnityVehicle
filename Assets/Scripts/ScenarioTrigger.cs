using System;
using System.Collections;
using GleyTrafficSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ScenarioTrigger : MonoBehaviour
{
    public PhysicarControl m_DecCar;
    public PlayerCar m_PlayerCar;
    private HUDManager m_HUDManager;
    public PhysicarControl[] m_SideCars;

    private void Start()
    {
        m_DecCar.gameObject.SetActive(true);
        m_HUDManager = gameObject.GetComponent<HUDManager>();
        foreach (var sideCar in m_SideCars)
        {
            sideCar.gameObject.SetActive(Random.value > 0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var body = other.GetComponentInParent<Rigidbody>();
        if (body && body.CompareTag("Player"))
        {
            m_DecCar.StartTrigger();
            m_PlayerCar.GetComponent<PhysicarControl>().enabled = false;
            m_PlayerCar.Drivable = true;
            m_HUDManager.ShowTakeoverRequest();
            
            StartCoroutine(DelayToLaunch());
        }
    }

    IEnumerator DelayToLaunch()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Launch");
    }
}