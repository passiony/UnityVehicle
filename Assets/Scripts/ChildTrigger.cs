using System;
using System.Collections;
using GleyTrafficSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ChildTrigger : MonoBehaviour
{
    [FormerlySerializedAs("m_Child")] public ChildDartOut[] m_Childs;
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
        m_FollowChild.SetTarget(m_TargetChild.transform);
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