using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEditor;

public static class AttentionReport
{
    public static float HitRate()
    {
        var s = AttentionStats.Instance;
        return (float)s.hitCount / Mathf.Max(1, s.totalNotes);
    }

    public static float ErrorRate()
    {
        var s = AttentionStats.Instance;
        return (float)s.badCount / Mathf.Max(1, s.totalNotes);
    }

    public static float MissRate()
    {
        var s = AttentionStats.Instance;
        return (float)s.missCount / Mathf.Max(1, s.totalNotes);
    }

    public static float AvgCombo()
    {
        var list = AttentionStats.Instance.comboHistory;
        return list.Count == 0 ? 0f : (float)list.Average();
    }

    public static int ComboBreakCount()
    {
        int count = 0;
        var list = AttentionStats.Instance.comboHistory;

        for (int i = 1; i < list.Count; i++)
        {
            if (list[i - 1] > 0 && list[i] == 0)
                count++;
        }
        return count;
    }
    public static float DprimeScore()
    {
        var score = Cdf(HitRate() / MissRate());
        return score;

    }

    public static int DprimeLevel()
    {
        var score = DprimeScore();

        if (ChartLoader.mode != ChartLoader.GameMode.Easy)
            score -= 0.5f;
        if (score <= 0.8f)
            return 0;
        else if (score <0.9f)
            return 1;
        else return 2;
    }
    public static float Pdf(float x)
    {
        return Mathf.Exp(-0.5f * x * x) / Mathf.Sqrt(2 * Mathf.PI);
    }

    /// <summary>
    /// CDF
    /// 使用误差函数erf近似
    /// </summary>
    public static float Cdf(float x)
    {
        // 使用误差函数erf 
        float z = x / Mathf.Sqrt(2);
        float t = 1.0f / (1.0f + 0.3275911f * Mathf.Abs(z));
        float erf = 1.0f - (((((
            1.061405429f * t +
            -1.453152027f) * t +
            1.421413741f) * t +
            -0.284496736f) * t +
            0.254829592f) * t) * Mathf.Exp(-z * z);

        float result = 0.5f * (1.0f + Mathf.Sign(z) * erf);

        // 处理负无穷大
        if (x <= -37) return 0;
        // 处理正无穷大
        if (x >= 37) return 1;

        return result;
    }
    public static void CreateReport(string levelName,int mode)
    {
        allReports.Add(new Report(DateTime.Now.ToShortTimeString(),levelName,mode));

    }
    public static void AddReportStamp(string ratingStr)
    {
        float h = HitRate();
        float m = MissRate();
        float e = ErrorRate();
        int max = AttentionStats.Instance.maxCombo;
        float a = AvgCombo();
        int b = ComboBreakCount();
        int dprime = DprimeLevel();
        float dpScore= DprimeScore();

        var p = new ReportStamp(h, m, e, max, a, b, dprime,dpScore,ratingStr); 
  
        allReports.Last().stamps.Add(p);
    }
    public static List<Report> allReports = new List<Report>();
}

public class Report
{
    public List<ReportStamp> stamps;
    public string time;
    public string levelName;
    public int mode;
    public Report(string t,string l,int m=-1)
    {
        time= t;
        levelName = l;
        mode = m;
        stamps = new List<ReportStamp>();
    }
}
public class ReportStamp
{


    public float hitRate, missRate, errorRate, avgCombo;
    public int maxCombo, breaks;
    public int dprime;
    public float avgReactionTime;
    public float dprimeScore;
    public string ratingStr;
    public ReportStamp(float h, float m, float e, int max, float avg, int b, int d,float dpScore,string ratingStr)
    {
        hitRate = h;
        missRate = m;
        errorRate = e;
        maxCombo = max;
        avgCombo = avg;
        breaks = b;
        dprime = d;

        dprimeScore = dpScore;
        this.ratingStr = ratingStr;
    }
}