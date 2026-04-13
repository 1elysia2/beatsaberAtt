[System.Serializable]
public class JudgmentSettings
{
    public float perfectTimeRange = 0.05f; // Perfect判定时间范围（秒）
    public float goodTimeRange = 0.1f;     // Good判定时间范围（秒）
    public float hitDistance = 2.0f;       // 判定平面前后的空间范围（米）
}
