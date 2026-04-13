using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReportMainScene : MonoBehaviour
{
    public Transform btnParent_reports, btnParent_stamps;
    public GameObject selector1,selector2;

    public GameObject btnPrefab;
    public Text hitRateText;
    public Text missRateText;
    public Text errorRateText;
    public Text maxComboText;
    public Text avgComboText;
    public Text breakCountText;
    public Text txt_dprimeScore;
    public Text txt_dprime;


    private void Clear1()
    {
        List<GameObject> des = new List<GameObject>();
        for (int i = 0; i < btnParent_reports.childCount; i++)
        {
            des.Add(btnParent_reports.GetChild(i).gameObject);
        }
        foreach (var d in des)
        {
            Destroy(d);
        }
        selector1.SetActive(false);
    }
    private void Clear2()
    {
        List<GameObject> des = new List<GameObject>();

        for (int i = 0; i < btnParent_stamps.childCount; i++)
        {
            des.Add(btnParent_stamps.GetChild(i).gameObject);
        }
        foreach (var d in des)
        {
            Destroy(d);
        }
        selector2.SetActive(false);
    }
 
    void OnEnable()
    {
        Clear1();
        Clear2();
 

        foreach (var v in AttentionReport.allReports)
        {
            var temp = v;
            var b = Instantiate(btnPrefab, btnParent_reports);
            b.GetComponentInChildren<Text>().text = temp.time + '\n' + temp.levelName + '\n' + (ChartLoader.GameMode)temp.mode;


            b.GetComponent<Button>().onClick.AddListener(() =>
            {

                selector1.SetActive(true);
                selector1.transform.position = b.transform.position;
                Clear2();
                for (int i = 0; i < temp.stamps.Count; i++)
                {
                    var stampBtn = Instantiate(btnPrefab, btnParent_stamps);
                    stampBtn.GetComponentInChildren<Text>().text = $"第{i + 1}个60秒";

                    var tempStamp = temp.stamps[i];
                    stampBtn.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        selector2.SetActive(true);
                        selector2.transform.position= stampBtn.transform.position;
                        int dp =tempStamp.dprime;
                        string hitRateStr = $"命中率: {tempStamp.hitRate:P1}";
                        string missRateStr = $"漏击率: {tempStamp.missRate:P1}";
                        string errorRateStr = $"错误率: {tempStamp.errorRate:P1}";
                        string maxComboStr = $"最大连击: {tempStamp.maxCombo}";
                        string avgComboStr = $"平均连击: {tempStamp.avgCombo:F1}";
                        string breakStr = $"注意力中断次数: {tempStamp.breaks}";
                        string dpStr = "D-prime: " +tempStamp.dprimeScore.ToShortString();
                        string dps = txt_dprime.text = "综合评估，您本次游玩的注意力结果为: " + (dp == 0 ? "差" : dp == 1 ? "中" : "优");
                        hitRateText.text = hitRateStr;
                        missRateText.text = missRateStr;
                        errorRateText.text = errorRateStr;
                        maxComboText.text = maxComboStr;
                        avgComboText.text = avgComboStr;
                        breakCountText.text = breakStr;
                        txt_dprimeScore.text = dpStr;
                        txt_dprime.text = dps;
                    });
                }

            });
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(btnParent_reports as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(btnParent_stamps as RectTransform);

    }
}
