using UnityEngine;

public class FollowChild : MonoBehaviour
{
    // 目标物体的Transform组件
    public Transform target;
    
    // 存储初始相对位置偏移
    private Vector3 initialPositionOffset;

    void Start()
    {
        if (target != null)
        {
            // 计算并存储初始相对位置偏移
            initialPositionOffset = transform.position - target.position;
        }
        else
        {
            Debug.LogWarning("Target not assigned to FollowChild script on " + gameObject.name);
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 更新位置：目标位置 + 初始相对位置偏移
            var targetPos = target.position + initialPositionOffset;
            targetPos.y = transform.position.y;
            transform.position = targetPos;
        }
    }
}