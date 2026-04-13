using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightsaberTrail : MonoBehaviour
{
    [Header("拖尾设置")]
    public float trailWidth = 0.1f;          // 拖尾宽度
    public Color trailColor = Color.cyan;    // 拖尾颜色
    public float trailDuration = 0.2f;       // 拖尾持续时间(秒)
    public int maxTrailPoints = 50;          // 最大拖尾点数

    [Header("速度阈值")]
    public float minSpeedForTrail = 1.0f;    // 触发拖尾的最小速度
    public float maxSpeedForTrail = 10.0f;   // 拖尾完全展开的最大速度

    private LineRenderer lineRenderer;
    private Vector3[] trailPositions;
    private int currentTrailPoint = 0;
    private bool isTrailActive = false;
    private Vector3 lastPosition;
    private float lastTime;

    void Start()
    {
        // 初始化LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = trailWidth;
        lineRenderer.endWidth = trailWidth;
        lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;

        // 初始化拖尾位置数组
        trailPositions = new Vector3[maxTrailPoints];
        for (int i = 0; i < maxTrailPoints; i++)
        {
            trailPositions[i] = transform.position;
        }

        lineRenderer.positionCount = maxTrailPoints;
        lineRenderer.SetPositions(trailPositions);

        lastPosition = transform.position;
        lastTime = Time.time;
    }

    void Update()
    {
        // 计算当前速度
        float speed = (transform.position - lastPosition).magnitude / (Time.time - lastTime);
        lastPosition = transform.position;
        lastTime = Time.time;


        // 根据速度决定是否激活拖尾
        float speedRatio = Mathf.Clamp01((speed - minSpeedForTrail) / (maxSpeedForTrail - minSpeedForTrail));

        if (speedRatio > 0.1f && !isTrailActive)
        {
            // 开始拖尾
            isTrailActive = true;
            currentTrailPoint = 0;
            lineRenderer.enabled = true;
        }
        else if (speedRatio <= 0.1f && isTrailActive)
        {
            // 结束拖尾
            isTrailActive = false;
            FadeOutTrail();
        }

        if (isTrailActive)
        {
            // 更新拖尾位置
            trailPositions[currentTrailPoint] = transform.position;
            currentTrailPoint = (currentTrailPoint + 1) % maxTrailPoints;

            // 更新LineRenderer
            UpdateLineRenderer();

            // 根据速度调整拖尾透明度
            Color color = trailColor;
            color.a = speedRatio;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    private void UpdateLineRenderer()
    {
        Vector3[] renderPositions = new Vector3[maxTrailPoints];

        // 从当前点开始，向后填充位置数据
        for (int i = 0; i < maxTrailPoints; i++)
        {
            int index = (currentTrailPoint + i) % maxTrailPoints;
            renderPositions[i] = trailPositions[index];
        }

        lineRenderer.SetPositions(renderPositions);
    }

    private void FadeOutTrail()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        float fadeTime = trailDuration;
        float elapsedTime = 0f;
        Color startColor = lineRenderer.startColor;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeTime);

            Color newColor = startColor;
            newColor.a = alpha;

            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;

            yield return null;
        }

        lineRenderer.enabled = false;
    }
}
