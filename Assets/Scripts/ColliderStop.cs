using System;
using System.Collections;
using System.Collections.Generic;
using GleyTrafficSystem;
using UnityEngine;

public class ColliderStop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        var body = other.gameObject.GetComponentInParent<Rigidbody>();
        if (body && body.CompareTag("Car"))
        {
            gameObject.GetComponent<PhysicarControl>().StartTrigger();
        }

        if (body && body.CompareTag("Child"))
        {
            body.constraints = RigidbodyConstraints.FreezeRotationY;
            body.GetComponent<ChildDartOut>().enabled = false;
            body.AddForce(body.transform.right * 100, ForceMode.Impulse);
            body.GetComponent<Animator>().Play("idle");
        }
    }
}