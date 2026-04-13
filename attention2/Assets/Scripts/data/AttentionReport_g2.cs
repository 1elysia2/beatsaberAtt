using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEditor;

public static class AttentionReport_g2
{
    public static float HitRate()
    {
        var s = AttentionStats_g2.Instance;
        return (float)s.hitCount / Mathf.Max(1, s.totalNotes);
    }

    public static float ErrorRate()
    {
        var s = AttentionStats_g2.Instance;
        return (float)s.badCount / Mathf.Max(1, s.totalNotes);
    }

    public static float MissRate()
    {
        var s = AttentionStats_g2.Instance;
        return (float)s.missCount / Mathf.Max(1, s.totalNotes);
    }

    public static float AvgCombo()
    {
        var list = AttentionStats_g2.Instance.comboHistory;
        return list.Count == 0 ? 0f : (float)list.Average();
    }
    public static float AvgReactionTime()
    {
        var s= AttentionStats_g2.Instance;
        return s.reactionTimeTotal/ Mathf.Max(1, s.hitCount);
    }
    public static int ComboBreakCount()
    {
        int count = 0;
        var list = AttentionStats_g2.Instance.comboHistory;

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

        if (score <= 1)
            return 0;
        else if (score < 2.5f)
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
    public static void CreateReport(string levelName)
    {
        allReports.Add(new Report(DateTime.Now.ToShortTimeString(),levelName));

    }
    public static void AddReportStamp(string ratingStr)
    {
        float h = HitRate();
        float m = MissRate();
        float e = ErrorRate();
        int max = AttentionStats_g2.Instance.maxCombo;
        float a = AvgCombo();
        int b = ComboBreakCount();
        int dprime = DprimeLevel();
        float dpScore= DprimeScore();

        var p = new ReportStamp(h, m, e, max, a, b, dprime,dpScore,ratingStr); 
        p.avgReactionTime = AvgReactionTime();
        allReports.Last().stamps.Add(p);
    }
    public static List<Report> allReports = new List<Report>();
}
