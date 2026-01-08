using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyArea : MonoBehaviour
{
    public Transform player;
    private GameObject plane;

    public float reminingWidth = 3;

    void Start()
    {
        plane = transform.GetChild(0).gameObject;
    }

    public void SetTarget(Transform target)
    {
        player = target;
        var pos = transform.position;
        pos.z = player.position.z;
        transform.position = pos;
    }

    void Update()
    {
        var offset = player.position - transform.position;
        offset.y = 0;
        offset.z = 0;
        var sign = Mathf.Sign(Vector3.Dot(transform.right, offset));
        var distance = offset.magnitude - reminingWidth;

        plane.SetActive(distance is > 3 and < 15 && sign > 0);
        transform.localScale = new Vector3(distance * sign, 1, 1);
    }
}