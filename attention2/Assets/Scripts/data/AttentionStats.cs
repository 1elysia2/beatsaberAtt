using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttentionStats : MonoBehaviour
{
    public static AttentionStats Instance;

    public int totalNotes;
    public int hitCount;
    public int missCount;
    public int badCount;

    [Header("�Ƿ���Ҫ������ٻ��������")]
    public bool isneedCalculate = false;
    public int maxCombo;
    public List<int> comboHistory = new List<int>();

    public bool isSongnew = false;      //�������ֹؿ��򲻼����ļ�

    private int lastCombo = 0;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ResetStats()
    {
        totalNotes = 0;
        hitCount = 0;
        missCount = 0;
        badCount = 0;
        maxCombo = 0;
        lastCombo = 0;

        comboHistory.Clear();
    }

    public void RecordHit(Judgment judgment)
    {
        totalNotes++;
        hitCount++;

        if (judgment == Judgment.Bad || judgment == Judgment.HoldBad)
            missCount++;
        if(judgment == Judgment.ErrorCube)
        badCount++;
        int currentCombo = Calculate.combo;
        comboHistory.Add(currentCombo);

        if (currentCombo > maxCombo)
            maxCombo = currentCombo;

        lastCombo = currentCombo;
    }

    public void RecordMiss()
    {
        totalNotes++;
        missCount++;
        comboHistory.Add(0);
        lastCombo = 0;
    }

    public void SavetoCSV(string freStr,string ratingStr)
    {
        if (LoginManager.Instance == null) return;
        if (isSongnew) return;

        LoginManager loginManager = LoginManager.Instance;
         loginManager.WriteCSVLine(new string[] { "" });
        loginManager.WriteCSVLine(new string[] { "TotalNotes", totalNotes.ToString() });
        loginManager.WriteCSVLine(new string[] { "HitCount", hitCount.ToString() });
        loginManager.WriteCSVLine(new string[] { "MissCount", missCount.ToString() });
        loginManager.WriteCSVLine(new string[] { "BadCount", badCount.ToString() });
        loginManager.WriteCSVLine(new string[] { "MaxCombe", maxCombo.ToString() });

        List<string> strings = new List<string> { "ComboHistory" };
        foreach (var a in comboHistory)
        {
            strings.Add(a.ToString());
        }
        loginManager.WriteCSVLine(strings.ToArray());
        loginManager.WriteCSVLine(new string[] { "生成频率",freStr });
        loginManager.WriteCSVLine(new string[] { "d-prime",AttentionReport.DprimeScore().ToShortString()});
loginManager.WriteCSVLine(new string[] { "评价",ratingStr});

        loginManager.WriteCSVLine(new string[] { "记录时间", DateTime.Now.ToString("yyyy-MM-dd_hh:mm:ss") });
    }

    public float Sometime()
    {
        if (!isneedCalculate) return 0;

        float sometime = 0f;

        sometime = missCount / totalNotes;

        if (sometime <= 0.3f)
        {
            sometime *= -3f;
        }
        else
        {
            sometime *= 3f;
        }

        Debug.Log($"��ǰ����Ϊ{sometime}");

        return sometime;
    }
}
