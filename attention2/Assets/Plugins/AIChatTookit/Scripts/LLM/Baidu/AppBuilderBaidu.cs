using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AppBuilderBaidu : LLM
{

    [Header("百度配置")]
    public string app_id = "";
    public string api_key = "";

    [Header("日志UI")]
    public Text logText;

    string logCache = "";

    string conversationID = "";
    string conversationUrl;

    bool conversationReady = false;



    void Awake()
    {
        Log("AI初始化");

        conversationUrl = "https://qianfan.baidubce.com/v2/app/conversation";
        url = "https://qianfan.baidubce.com/v2/app/conversation/runs";

        StartCoroutine(CreateConversation());
    }



    void Log(string msg)
    {
        Debug.Log(msg);

        logCache += "\n" + msg;

        if (logText != null)
            logText.text = logCache;
    }



    void LogError(string msg)
    {
        Debug.LogError(msg);

        logCache += "\n<color=red>" + msg + "</color>";

        if (logText != null)
            logText.text = logCache;
    }



    IEnumerator CreateConversation()
    {

        Log("开始创建会话");

        CreateConversationData data = new CreateConversationData();
        data.app_id = app_id;

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(conversationUrl, "POST"))
        {

            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Appbuilder-Authorization", "Bearer " + api_key);

            yield return request.SendWebRequest();


#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isHttpError || request.isNetworkError)
#endif
            {
                LogError("创建会话失败：" + request.error);
                yield break;
            }

            string result = request.downloadHandler.text;

            Log("会话返回：" + result);

            ConversationCreateResponse response =
                JsonUtility.FromJson<ConversationCreateResponse>(result);

            if (response == null)
            {
                LogError("会话解析失败");
                yield break;
            }

            conversationID = response.conversation_id;

            if (string.IsNullOrEmpty(conversationID))
            {
                LogError("conversationID为空");
                yield break;
            }

            Log("会话创建成功：" + conversationID);

            conversationReady = true;

        }

    }



    public override void PostMsg(string msg, Action<string> callback)
    {

        if (string.IsNullOrEmpty(msg))
        {
            LogError("消息为空");
            return;
        }

        StartCoroutine(SendMsg(msg, callback));
    }



    IEnumerator SendMsg(string msg, Action<string> callback)
    {

        while (!conversationReady)
        {
            Log("等待会话创建...");
            yield return null;
        }

        Log("发送消息：" + msg);

        yield return Request(msg, callback);
    }



    public override IEnumerator Request(string msg, Action<string> callback)
    {

        RequestData data = new RequestData();

        data.app_id = app_id;
        data.query = msg;
        data.conversation_id = conversationID;

        string json = JsonUtility.ToJson(data);

        Log("发送JSON：" + json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {

            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Appbuilder-Authorization", "Bearer " + api_key);

            yield return request.SendWebRequest();


#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isHttpError || request.isNetworkError)
#endif
            {
                LogError("网络错误：" + request.error);
                yield break;
            }

            string result = request.downloadHandler.text;

            Log("AI返回：" + result);

            ResponseData response =
                JsonUtility.FromJson<ResponseData>(result);

            if (response == null)
            {
                LogError("AI解析失败");
                yield break;
            }

            Log("AI回答：" + response.answer);

            callback?.Invoke(response.answer);

        }

    }



    [Serializable]
    public class CreateConversationData
    {
        public string app_id;
    }



    [Serializable]
    public class ConversationCreateResponse
    {
        public string request_id;
        public string conversation_id;
        public int code;
        public string message;
    }



    [Serializable]
    public class RequestData
    {
        public string app_id;
        public string query;
        public bool stream = false;
        public string conversation_id;
    }



    [Serializable]
    public class ResponseData
    {
        public int code;
        public string message;
        public string answer;
        public string conversation_id;
    }

}