using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G3RayCast : MonoBehaviour
{
    // 检测范围，默认为50米
    public float detectionRange = 50f;

    // 射线的起始位置偏移量
    public Vector3 rayOriginOffset = Vector3.zero;

    // 用于控制mesh显示的组件
    private MeshRenderer meshRenderer;

    // 用于调试的射线颜色
    public Color rayColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        // 获取物体的MeshRenderer组件
        meshRenderer = GetComponent<MeshRenderer>();

        // 如果没有MeshRenderer组件，尝试获取子物体的MeshRenderer
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 计算射线的起始位置
        Vector3 rayOrigin = transform.position + transform.TransformDirection(rayOriginOffset);

        // 计算射线的方向（物体的前方）
        Vector3 rayDirection = transform.forward;

        // 存储碰撞信息
        RaycastHit hitInfo;

        int layermask = LayerMask.GetMask("Vehicles");
        // 发射射线
        bool hasCollision = Physics.Raycast(rayOrigin, rayDirection, out hitInfo, detectionRange, layermask);

        // 在Scene视图中绘制射线，用于调试
        Debug.DrawRay(rayOrigin, rayDirection * detectionRange, rayColor);

        // 根据检测结果控制mesh的显示和隐藏
        if (meshRenderer != null)
        {
            meshRenderer.enabled = !hasCollision;
        }

        // 打印调试信息
        if (hasCollision)
        {
            Debug.Log("检测到碰撞，碰撞物体: " + hitInfo.collider.gameObject.name);
        }
        else
        {
            Debug.Log("未检测到碰撞");
        }
    }
}