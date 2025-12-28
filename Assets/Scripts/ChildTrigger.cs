using System;
using System.Collections;
using GleyTrafficSystem;
using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    public ChildDartOut m_Child;
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
            m_Child.StartTrigger();
            m_PlayerCar.GetComponent<PhysicarControl>().enabled = false;
            m_PlayerCar.Drivable = true;
            m_HUDManager.ShowTakeoverRequest();
        }
    }
}
