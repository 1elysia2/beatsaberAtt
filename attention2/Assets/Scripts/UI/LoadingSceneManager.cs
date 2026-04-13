// LoadingScreenManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public static string nextSceneName; // 要加载的场景名
    public static int nextSceneIndex; // 要加载的场景名

    [SerializeField]
    private Slider loadingBar;
    public Text loadingText;

    void Start()
    {
        Debug.Log("indx" + nextSceneIndex);
        StartCoroutine(LoadNextScene());
        loadingText.gameObject.SetActive(false);
    }

    public IEnumerator LoadNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false; // 禁止自动跳转

        // 更新进度条（0-0.9范围）
        while (asyncLoad.progress < 0.9f)
        {
            loadingBar.value = asyncLoad.progress;
            yield return null;
        }

        // 等待加载完成（此时progress=0.9）
        loadingBar.value = 1f; // 强制显示满进度

        loadingBar.gameObject.SetActive(false);

        loadingText.gameObject.SetActive(true);

        // 等待玩家确认（可选）
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        asyncLoad.allowSceneActivation = true; // 激活场景
    }
}