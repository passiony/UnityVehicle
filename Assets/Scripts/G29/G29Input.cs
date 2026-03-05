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
        playerCar.SetSteering(_steerVal);
        if (_brakeVal < -0.1)
        {
            // playerCar.SetMortor((_brakeVal - 1) * 0.5f);
            playerCar.SetMortor(0);
        }
        else if (_throttleVal < -0.1)
        {
            playerCar.SetMortor((1 - _throttleVal) * 0.5f);
        }
    }
}