using UnityEngine;
using UnityEngine.InputSystem;

public class G29Input : MonoBehaviour
{
    // 1. 输入动作引用
    public InputActionProperty steering;
    public InputActionProperty throttle;
    public InputActionProperty brake;

    // 2. 输入值缓存（优化性能）
    [SerializeField]
    private float _steerVal;
    [SerializeField]
    private float _throttleVal;
    [SerializeField]
    private float _brakeVal;

    // 3. 力反馈参数
    [Header("力反馈设置")] [Range(0, 1)] public float roadVibration = 0.5f;
    [Range(0, 1)] public float brakeVibration = 0.7f;

    void OnEnable()
    {
        // 启用输入动作
        steering.action.Enable();
        throttle.action.Enable();
        brake.action.Enable();

        // 4. 注册回调事件（替代Update轮询）
        steering.action.performed += ctx => _steerVal = ctx.ReadValue<float>();
        throttle.action.performed += ctx => _throttleVal = ctx.ReadValue<float>();
        brake.action.performed += ctx => _brakeVal = ctx.ReadValue<float>();
    }

    void OnDisable()
    {
        steering.action.Disable();
        throttle.action.Disable();
        brake.action.Disable();
    }

    void Update()
    {
        // 6. 应用输入到车辆物理系统
        ApplySteering(_steerVal);
        ApplyThrottle(_throttleVal);
        ApplyBrake(_brakeVal);
    }

    // 9. 力反馈更新逻辑
    private void ApplyBrake(float brakeVal)
    {
        // 基础路面震动 + 刹车增强震动
        // float vibration = roadVibration + (brakeVal > 0.1f ? brakeVibration : 0);
        Debug.Log("Brake:" + brakeVal);
    }

    // 示例车辆控制方法（需替换为实际物理逻辑）
    private void ApplySteering(float value)
    {
        Debug.Log("Steering:" + value);
        // transform.Rotate(Vector3.up, value * Time.deltaTime * 100f);
    }

    private void ApplyThrottle(float value)
    {
        Debug.Log("Throttle:" + value);
        // transform.Translate(Vector3.forward * value * Time.deltaTime * 5f);
    }
}