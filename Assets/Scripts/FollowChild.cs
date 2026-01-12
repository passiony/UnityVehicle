using System;
using UnityEngine;

public class FollowChild : MonoBehaviour
{
    // 目标物体的Transform组件
    public Transform player;
    public Transform roadCenter;

    private GameObject mesh;
    // 存储初始相对位置偏移
    public Vector3 Offset;

    private void Awake()
    {
        mesh = transform.GetChild(0).gameObject;
    }

    public void SetTarget(Transform target,int index)
    {
        player = target;
        if (index == 0)
        {
            Offset.x = 1;
        }
        if (index == 1)
        {
            Offset.x = -1;
        }
    }
    
    void LateUpdate()
    {
        if (player != null)
        {
            // 更新位置：目标位置 + 初始相对位置偏移
            var targetPos = player.position + Offset;
            targetPos.y = transform.position.y;
            transform.position = targetPos;
        }
        // var offset  = roadCenter.position - player.position;
        // offset.z = 0;
        // mesh.SetActive(offset.magnitude <= 10);
    }
}