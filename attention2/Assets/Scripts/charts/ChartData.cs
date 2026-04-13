using System.Collections.Generic;


[System.Serializable]
public class ChartData
{
    public float bpm;      // 音乐速度
    public float offset;   // 音乐与谱面的时间偏移（校准用）
    public List<NoteData> notes;
}

[System.Serializable]
public class NoteData
{
    public float time;     // 音符应被击打的时间（单位：秒）
    public int lane;       // 轨道编号（0=左，1=右）
    public string direction; // 方向字段
    public string noteType = "tap"; //类型（tap/hold）
    public float duration;    //hold持续时间
}

