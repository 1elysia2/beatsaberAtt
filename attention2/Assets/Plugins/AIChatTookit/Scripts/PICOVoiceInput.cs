// PicoVoiceInputs.cs 最终适配版
using UnityEngine;
using UnityEngine.Android;

public class PicoVoiceInputs : MonoBehaviour
{
    [Header("录音设置")]
    public int maxRecordingLength = 15; // 最大录音时长(秒)
    public int sampleRate = 16000;     // 采样率

    private AudioClip recordingClip;
    private bool isRecording = false;

    public void StartRecordAudio()
    {
        if (isRecording) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            return;
        }
#endif

        recordingClip = Microphone.Start(null, false, maxRecordingLength, sampleRate);
        isRecording = true;
        Debug.Log("Pico麦克风开始录音");
    }

    public void StopRecordAudio(System.Action<AudioClip> callback)
    {
        if (!isRecording) return;

        Microphone.End(null);
        isRecording = false;

        // 返回有效录音片段
        callback?.Invoke(recordingClip);
    }

    void OnDestroy()
    {
        if (isRecording)
            Microphone.End(null);
    }
}
