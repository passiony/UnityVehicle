using System.Collections;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    // 目标材质
    private Material targetMaterial;

    // 渐变时间（秒）
    public float fadeDuration = 4.0f;

    // 开始透明度（0 = 完全透明）
    private float startAlpha = 0.0f;

    // 结束透明度（1 = 完全不透明）
    private float endAlpha = 0.6f;

    void Start()
    {
        // 获取目标材质（假设是渲染器的第一个材质）
        Renderer renderer = GetComponent<Renderer>();
        targetMaterial = renderer.material;
        Color currentColor = targetMaterial.color;
        currentColor.a = startAlpha;
        targetMaterial.color = currentColor;

        // 自动开始淡入
        StartFadeIn();
    }

    // 开始淡入协程
    public void StartFadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    // 淡入协程
    private IEnumerator FadeInCoroutine()
    {
        if (targetMaterial == null || !targetMaterial.HasProperty("_Color"))
        {
            yield break;
        }

        float elapsedTime = 0.0f;
        Color currentColor = targetMaterial.color;

        while (elapsedTime < fadeDuration)
        {
            // 计算当前透明度（使用线性插值）
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

            // 更新材质颜色的alpha通道
            currentColor.a = alpha;
            targetMaterial.color = currentColor;

            // 更新经过的时间
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 确保最终透明度为1
        currentColor.a = endAlpha;
        targetMaterial.color = currentColor;
    }
}