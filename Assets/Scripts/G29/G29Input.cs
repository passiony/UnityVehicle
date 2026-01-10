using System;
using GleyTrafficSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class G29Input : MonoBehaviour
{
    PlayerCar playerCar;

    // 1. 输入动作引用
    public InputActionProperty steering;
    public InputActionProperty throttle;
    public InputActionProperty brake;

    // 2. 输入值缓存（优化性能）
    [SerializeField] private float _steerVal;
    [SerializeField] private float _throttleVal;
    [SerializeField] private float _brakeVal;

    public float SteerVal => _steerVal;
    public float ThrottleVal => _throttleVal;
    public float BrakeVal => _brakeVal;
    
    // 3. 力反馈参数
    [Header("力反馈设置")] [Range(0, 1)] public float roadVibration = 0.5f;
    [Range(0, 1)] public float brakeVibration = 0.7f;

    private void Awake()
    {
        playerCar = gameObject.GetComponent<PlayerCar>();
    }

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
        playerCar.SetSteering(_steerVal * 0.25f);
        var brake = 1 - _brakeVal;
        if (brake >= 0.01f)
        {
            playerCar.SetMortor(-_brakeVal);
        }
        else
        {
            playerCar.SetMortor(1 - _throttleVal);
        }
    }
}