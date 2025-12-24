using UnityEngine;

/// <summary>
/// 小孩突然窜出
/// </summary>
public class ChildDartOut : MonoBehaviour
{
    private Rigidbody rb;
    private bool isRunning = false;

    [Header("设置")]
    public float runSpeed = 5.0f;     // 小孩跑动速度 (m/s)
    public Vector3 runDirection = Vector3.left; // 窜出的方向（根据你的马路方向设置）

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 初始状态下，确保小孩不会因为重力滑走
        rb.isKinematic = true; 
    }

    void FixedUpdate()
    {
        if (isRunning)
        {
            // 使用 MovePosition 保证物理碰撞有效
            // 它是物理平移，不会像 Translate 那样穿墙
            Vector3 nextPos = rb.position + transform.TransformDirection(runDirection) * runSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);
        }
    }

    // 外部调用：开始窜出
    public void StartTrigger()
    {
        if (!isRunning)
        {
            isRunning = true;
            rb.isKinematic = false;
            
            // 如果你有动画组件，这里可以播放跑步动画
            // GetComponent<Animator>().SetTrigger("Run"); 

            Debug.Log("小孩窜出！触发危险时刻。");
        }
    }
}