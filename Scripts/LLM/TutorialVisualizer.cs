using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialVisualizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private XRRayInteractor leftRayInteractor;
    [SerializeField] private XRRayInteractor rightRayInteractor;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private AudioSource voiceOverSource;

    [Header("Visual Cues")]
    [SerializeField] private GameObject leftSaberHighlight;
    [SerializeField] private GameObject rightSaberHighlight;
    [SerializeField] private GameObject directionArrowPrefab;

    public void DisplayTutorialMessage(string message)
    {
        // 显示文本
        tutorialText.text = message;
        tutorialPanel.SetActive(true);

        // 语音播报
        if (voiceOverSource.isPlaying) voiceOverSource.Stop();
        // 这里可以集成TTS服务或播放预录制的音频

        // 解析特殊指令（示例）
        if (message.Contains("left saber"))
        {
            HighlightSaber(leftSaberHighlight, true);
        }
        else if (message.Contains("right saber"))
        {
            HighlightSaber(rightSaberHighlight, true);
        }

        // 3秒后自动隐藏
        CancelInvoke(nameof(HideTutorial));
        Invoke(nameof(HideTutorial), 5f);
    }

    private void HighlightSaber(GameObject saber, bool highlight)
    {
        saber.SetActive(highlight);
        if (highlight)
        {
            CancelInvoke(nameof(DisableSaberHighlight));
            Invoke(nameof(DisableSaberHighlight), 3f);
        }
    }

    // 新添加的辅助方法
    private void DisableSaberHighlight()
    {
        if (leftSaberHighlight != null) leftSaberHighlight.SetActive(false);
        if (rightSaberHighlight != null) rightSaberHighlight.SetActive(false);
    }

    private void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void ShowDirectionIndicator(Vector3 direction)
    {
        var arrow = Instantiate(directionArrowPrefab, transform);
        arrow.transform.forward = direction;
        Destroy(arrow, 3f);
    }
}
