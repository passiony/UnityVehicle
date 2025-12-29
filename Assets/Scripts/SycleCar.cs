using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SycleCar : MonoBehaviour
{
    public Transform syclePoint;
    
    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<PhysicarControl>();
        if (car)
        {
            car.transform.position = syclePoint.position;
        }
    }
}
