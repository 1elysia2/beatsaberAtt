using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReportPanelController : MonoBehaviour
{
    public Text hitRateText;
    public Text missRateText;
    public Text errorRateText;
    public Text maxComboText;
    public Text avgComboText;
    public Text breakCountText;
    public Text reactionTimeText;
    public Text txt_dprimeScore;
    public Text txt_dprime;

    public GameObject panel;
    void OnEnable()
    {
        ShowReport();
    }

    public void ShowReport()
    {
        panel.SetActive(true);

        if (!SceneManager.GetActiveScene().name.Contains("g2"))
        {
            // 1️⃣ 先把“原始数值”打出来（不格式化）
            Debug.Log($"[RAW] HitRate = {AttentionReport.HitRate()}");
            Debug.Log($"[RAW] MissRate = {AttentionReport.MissRate()}");
            Debug.Log($"[RAW] ErrorRate = {AttentionReport.ErrorRate()}");
            Debug.Log($"[RAW] MaxCombo = {AttentionStats.Instance.maxCombo}");
            Debug.Log($"[RAW] AvgCombo = {AttentionReport.AvgCombo()}");
            Debug.Log($"[RAW] BreakCount = {AttentionReport.ComboBreakCount()}");

            // 2️⃣ 再把“格式化后的字符串”打出来
            string hitRateStr = $"命中率: {AttentionReport.HitRate():P1}";
            string missRateStr = $"漏击率: {AttentionReport.MissRate():P1}";
            string errorRateStr = $"错误率: {AttentionReport.ErrorRate():P1}";
            string maxComboStr = $"最大连击: {AttentionStats.Instance.maxCombo}";
            string avgComboStr = $"平均连击: {AttentionReport.AvgCombo():F1}";
            string breakStr = $"注意力中断次数: {AttentionReport.ComboBreakCount()}";


            int dp = AttentionReport.DprimeLevel();
            Debug.Log($"[STR] {hitRateStr}");
            Debug.Log($"[STR] {missRateStr}");
            Debug.Log($"[STR] {errorRateStr}");
            Debug.Log($"[STR] {maxComboStr}");
            Debug.Log($"[STR] {avgComboStr}");
            Debug.Log($"[STR] {breakStr}");

            // 3️⃣ 最后才真正赋值给 UI
            hitRateText.text = hitRateStr;
            missRateText.text = missRateStr;
            errorRateText.text = errorRateStr;
            maxComboText.text = maxComboStr;
            avgComboText.text = avgComboStr;
            breakCountText.text = breakStr;
            txt_dprimeScore.text = "D-prime: " + AttentionReport.DprimeScore().ToShortString();
            txt_dprime.text = "综合评估，您本次游玩的注意力结果为: " + (dp == 0 ? "差" : dp == 1 ? "中" : "优");

        }
        else
        {
            string hitRateStr = $"命中率: {AttentionReport_g2.HitRate():P1}";
            string missRateStr = $"漏击率: {AttentionReport_g2.MissRate():P1}";
            string errorRateStr = $"错误率: {AttentionReport_g2.ErrorRate():P1}";
            string maxComboStr = $"最大连击: {AttentionStats_g2.Instance.maxCombo}";
            string avgComboStr = $"平均连击: {AttentionReport_g2.AvgCombo():F1}";
            string breakStr = $"注意力中断次数: {AttentionReport_g2.ComboBreakCount()}";
            string reactionTimeStr = $"平均反应时间:{AttentionReport_g2.AvgReactionTime()}";
            int dp = AttentionReport_g2.DprimeLevel();
            hitRateText.text = hitRateStr;
            missRateText.text = missRateStr;
            errorRateText.text = errorRateStr;
            maxComboText.text = maxComboStr;
            avgComboText.text = avgComboStr;
            reactionTimeText.text = reactionTimeStr;
            breakCountText.text = breakStr;
            txt_dprimeScore.text = "D-prime: " + AttentionReport_g2.DprimeScore().ToShortString();
            txt_dprime.text = "综合评估，您本次游玩的注意力结果为: " + (dp == 0 ? "差" : dp == 1 ? "中" : "优");
        }





    }

}
