using System;
using System.Collections;
using GleyTrafficSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildTrigger : MonoBehaviour
{
    public ChildDartOut[] m_Child;
    public PlayerCar m_PlayerCar;
    private HUDManager m_HUDManager;
    
    void Start()
    {
        m_HUDManager = gameObject.GetComponent<HUDManager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var body = other.GetComponentInParent<Rigidbody>();
        if (body && body.CompareTag("Player"))
        {
            foreach (var child in m_Child)
            {
                child.StartTrigger();
            }
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
