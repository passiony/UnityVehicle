using UnityEngine;

/// <summary>
/// 前车突然减速
/// </summary>
public class CarPhysicsControl : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("实验设置")]
    public float initialSpeedKmh = 60f; // 初始速度 (km/h)
    public float targetDeceleration = 6.0f; // 减速度 (m/s^2)

    private float currentTargetSpeedMps; // 目标速度 (m/s)
    private bool isBraking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 1. 将 km/h 转换为 m/s (公式：除以 3.6)
        currentTargetSpeedMps = initialSpeedKmh / 3.6f;

        // 2. 立即赋予初始速度，防止从 0 开始爬升
        rb.velocity = transform.forward * currentTargetSpeedMps;
    }

    void FixedUpdate()
    {
        if (!isBraking)
        {
            // 巡航模式：维持初始速度，抵消摩擦力
            MaintainSpeed(currentTargetSpeedMps);
        }
        else
        {
            // 减速模式：执行 -6m/s^2 的减速
            ApplyDeceleration();
        }
    }

    // 维持速度的逻辑（模拟自动驾驶巡航）
    void MaintainSpeed(float targetSpeed)
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        localVel.z = targetSpeed; // 强制保持 Z 轴速度恒定
        rb.velocity = transform.TransformDirection(localVel);
    }

    // 触发减速的逻辑
    void ApplyDeceleration()
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        
        if (localVel.z > 0.1f)
        {
            // v = v0 - a * t
            localVel.z -= targetDeceleration * Time.fixedDeltaTime;
            localVel.z = Mathf.Max(0, localVel.z); // 防止倒车
            rb.velocity = transform.TransformDirection(localVel);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    // 外部接口：由触发器调用
    public void StartTrigger()
    {
        isBraking = true;
    }
}