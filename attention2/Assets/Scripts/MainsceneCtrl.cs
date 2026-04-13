using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainsceneCtrl : MonoBehaviour
{
    public GameObject UI_0;
    public GameObject UI_1;

    public InputField InputField;
    public Text errortext;

    private void OnEnable()
    {
        if(LoginManager.Instance != null)
        {
            if(LoginManager.Instance.islogin)
            {
                UI_0?.SetActive(false);
                UI_1?.SetActive(true);
            }
            else
            {
                UI_0?.SetActive(true);
                UI_1?.SetActive(false);
            }
        }
    }
    //登录界面
    public void Login()
    {
        if (LoginManager.Instance == null) return;
        if (InputField == null) return;

        string inputId = InputField.text.Trim(); // ȥ����β�ո�
        if (string.IsNullOrEmpty(inputId))
        {
            errortext.gameObject.SetActive(true);
            Invoke(nameof(Hideerrortext), 3f);
            return;
        }

        UI_0.SetActive(false);
        UI_1.SetActive(true);
        // ���ô����û���Ϣ
        LoginManager.Instance.SetUserID(inputId);

        InputField.text = string.Empty;
    }

    public void SetLoginFalse()
    {
        if(LoginManager.Instance != null)
        {
            LoginManager.Instance.islogin = false;
        }
    }

    void Hideerrortext()
    {
        errortext.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //�л��������Ҹ����л��ĳ��������û��ļ���д���ʼ��Ϣ
    public void LoadingScene(int index)     
    {
        if(LoginManager.Instance == null) return;

        LoginManager.Instance.CreateUserMessage();
        LoginManager.Instance.WriteCSVLine(new string[] {"" });         //��һ��
        
        if(index == 1)
        {
            ChartLoader.levelName = "songnew";
        }
        SceneManager.LoadScene(index,LoadSceneMode.Single);
    }
}
