using UnityEngine;
using UnityEngine.Android;
using System;

public class SpeechToTextManager : MonoBehaviour
{
    // 原有字段
    private bool isDictating = false;
    [SerializeField] private LLMInteractionManager llmManager;
    private AndroidJavaObject speechRecognizer;

    // 关键词唤醒词
    private readonly string[] keywords = new string[] { "Hey PICO", "Help me", "How to play" };

    void Start()
    {
        // 1. 关键词唤醒初始化
        RequestMicrophonePermission();
        Debug.Log("SpeechToTextManager Initialized (Android Mode)");
    }

    private void RequestMicrophonePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    // 2. 连续语音听写
    public void StartDictation()
    {
        if (isDictating) return;

        try
        {
            AndroidJavaClass recognizerClass = new AndroidJavaClass("android.speech.SpeechRecognizer");
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity");

            speechRecognizer = recognizerClass.CallStatic<AndroidJavaObject>("createSpeechRecognizer", activity);
            speechRecognizer.Call("setRecognitionListener", new AndroidListener(this));

            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            intent.Call<AndroidJavaObject>("setAction", "android.speech.action.RECOGNIZE_SPEECH");
            intent.Call<AndroidJavaObject>("putExtra", "android.speech.extra.LANGUAGE_MODEL", "free_form");

            speechRecognizer.Call("startListening", intent);
            isDictating = true;
            Debug.Log("Dictation started (Android)");
        }
        catch (Exception e)
        {
            Debug.LogError("Android SpeechRecognizer Error: " + e.Message);
        }
    }

    public void StopDictation()
    {
        if (!isDictating || speechRecognizer == null) return;

        speechRecognizer.Call("stopListening");
        isDictating = false;
        Debug.Log("Dictation stopped (Android)");
    }

    // 3. 处理识别结果
    private void OnDictationResult(string text)
    {
        Debug.Log("User said: " + text);

        // 关键词唤醒检测
        foreach (string keyword in keywords)
        {
            if (text.Contains(keyword))
            {
                Debug.Log($"Wake word detected: {keyword}");
                return;
            }
        }

        // 传递给LLM
        llmManager.ProcessUserQuery(text);
    }

    // 4. 资源释放
    void OnDestroy()
    {
        if (speechRecognizer != null)
        {
            speechRecognizer.Call("destroy");
            Debug.Log("SpeechRecognizer resources released");
        }
    }

    // Android回调监听
    private class AndroidListener : AndroidJavaProxy
    {
        private SpeechToTextManager parent;
        public AndroidListener(SpeechToTextManager parent) : base("android.speech.RecognitionListener")
        {
            this.parent = parent;
        }

        public void onResults(AndroidJavaObject results)
        {
            AndroidJavaObject matches = results.Call<AndroidJavaObject>("getStringArrayList", "results_recognition");
            if (matches != null)
            {
                string[] texts = AndroidJNIHelper.ConvertFromJNIArray<string[]>(matches.GetRawObject());
                if (texts.Length > 0) parent.OnDictationResult(texts[0]);
            }
            parent.isDictating = false;
        }

        public void onError(int error)
        {
            Debug.LogError($"Dictation error: {error}");
            parent.isDictating = false;
        }

        // 空实现（保持接口兼容性）
        public void onReadyForSpeech(AndroidJavaObject bundle) { }
        public void onBeginningOfSpeech() { }
        public void onEndOfSpeech() { }
    }
}
