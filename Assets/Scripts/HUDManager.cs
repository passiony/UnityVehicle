using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("G1: 基础警报")]
    public GameObject steeringWheelIcon; // 红色方向盘图标
    public AudioSource voiceAlarm;       // “请立即接管”语音
    private bool isFlashing = false;

    [Header("G2: 风险可视化")]
    public GameObject riskZonePrefab;    // 红色风险区域（带Projector或平面的Prefab）
    private GameObject activeRiskZone;

    [Header("G3: 增强态势理解")]
    public GameObject[] safetyZones;     // 绿色安全区域数组（左、中、右车道）

    public enum HUDLevel { G1, G2, G3 }
    public HUDLevel currentLevel = HUDLevel.G1;

    public void ShowTakeoverRequest(Transform dangerTarget)
    {
        // 1. 触发 G1 内容
        ActivateG1();

        // 2. 根据等级触发增强内容
        if (currentLevel == HUDLevel.G2 || currentLevel == HUDLevel.G3)
        {
            ActivateG2(dangerTarget);
        }

        if (currentLevel == HUDLevel.G3)
        {
            ActivateG3();
        }
    }

    public void HideAllHUD()
    {
        isFlashing = false;
        steeringWheelIcon.SetActive(false);
        if(activeRiskZone) activeRiskZone.SetActive(false);
        foreach (var zone in safetyZones) zone.SetActive(false);
    }

    // G1播放语音
    private void ActivateG1()
    {
        steeringWheelIcon.SetActive(true);
        isFlashing = true;
        voiceAlarm.Play(); 
        StartCoroutine(FlashSteeringWheel());
    }

    // G2 将红色风险区放置在危险源（前车或行人）位置
    private void ActivateG2(Transform target)
    {
        
        if (activeRiskZone == null) activeRiskZone = Instantiate(riskZonePrefab);
        activeRiskZone.transform.SetParent(target); // 随目标移动
        activeRiskZone.transform.localPosition = new Vector3(0, 0.05f, 0); // 贴近地面
        activeRiskZone.SetActive(true);
    }

    // G3 逻辑：检测车道是否安全。这里简单演示为激活所有预置绿色区域
    // 在实际项目中，你可以加一个Raycast判断左/右是否有车
    private void ActivateG3()
    {
        foreach (var zone in safetyZones)
        {
            zone.SetActive(true);
        }
    }

    // 方向盘图标闪烁协程
    IEnumerator FlashSteeringWheel()
    {
        Image img = steeringWheelIcon.GetComponent<Image>();
        while (isFlashing)
        {
            img.enabled = !img.enabled;
            yield return new WaitForSeconds(0.4f); // 闪烁频率
        }
    }
}