using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System;

public class LLMInteractionManager : MonoBehaviour
{
    [SerializeField] private TutorialVisualizer visualizer;
    [SerializeField] private string apiEndpoint = "https://api.your-llm-service.com/v1/chat/completions";
    [SerializeField] private string apiKey = "your-api-key";

    private HttpClient httpClient = new HttpClient();
    private string conversationContext = "";

    void Start()
    {
        // 初始化教程上下文
        conversationContext = "You are a friendly VR rhythm game tutor for Beat Saber. " +
                             "Provide short, clear instructions (under 20 words) with visual cues. " +
                             "Current game version: 1.0. User is a beginner.";
    }

    public async void ProcessUserQuery(string userInput)
    {
        try
        {
            // 更新对话上下文
            conversationContext += $"\nUser: {userInput}";

            // 构造LLM请求
            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = conversationContext },
                    new { role = "user", content = userInput }
                },
                max_tokens = 50
            };

            string jsonData = JsonUtility.ToJson(requestData);
            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            // 发送请求
            var response = await httpClient.PostAsync(apiEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            // 解析响应
            var responseData = JsonUtility.FromJson<LLMResponse>(responseString);
            string aiResponse = responseData.choices[0].message.content;

            // 更新上下文并显示
            conversationContext += $"\nAI: {aiResponse}";
            visualizer.DisplayTutorialMessage(aiResponse);
        }
        catch (Exception e)
        {
            Debug.LogError($"LLM error: {e.Message}");
            visualizer.DisplayTutorialMessage("Sorry, I didn't catch that. Could you repeat?");
        }
    }

    [System.Serializable]
    private class LLMResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string content;
    }
}
