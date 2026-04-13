using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public static LoginManager _instance;
    public static LoginManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("?????")]
    public bool islogin;
    public string UserID = "";
    private string UserFlieName = "";

    public void SetUserID(string userid)
    {
        UserID = userid;
    }

    /// <summary>
    /// CSV文件管理
    /// </summary>
    /// <param name="userid">ID</param>
    public void CreateUserMessage()
    { 
        UserFlieName = $"{UserID}";

        CreateCSVFile();
    }

    // ????????????CSV???��??
    private string GetCSVFilePath(string fileName)
    {
        string folderPath = string.Empty;

#if UNITY_EDITOR
        folderPath = Application.streamingAssetsPath;
#elif UNITY_ANDROID
        folderPath = Path.Combine(Application.persistentDataPath, "CSVFiles_PlayerDatas");
#endif

        // ????????????????????try-catch??
        try
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log($"???????????{folderPath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"??????????{e.Message}");
            return "";
        }

        return Path.Combine(folderPath, $"{fileName}.csv");
    }

    /// <summary>
    /// ????CSV????????????��??????????
    /// </summary>
     void CreateCSVFile()
    {
        // header????????????????????
        string header = "";

        if (string.IsNullOrWhiteSpace(UserFlieName))
        {
            Debug.LogError("?????????��????????");
            return;
        }

        string csvPath = GetCSVFilePath(UserFlieName);
        //��???????????
        if (string.IsNullOrEmpty(csvPath))
        {
            Debug.LogError("CSV???��??????????");
            return;
        }

        try
        {
            if (!File.Exists(csvPath))
            {
                // ??File.WriteAllText??????????????????????
                File.WriteAllText(csvPath, header + Environment.NewLine, Encoding.UTF8);
                Debug.Log($"CSV????????????{csvPath}");

                //��???????
                WriteCSVLine(new string[] { "用户id", UserID });
                WriteCSVLine(new string[] { "" });         //?????
                WriteCSVLine(new string[] { "用户初始创建时间" ,DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")});
                WriteCSVLine(new string[] { "" });         //?????

                islogin = true; // ?????/???????
            }
            else
            {
                Debug.Log($"CSV?????????{csvPath}");
                islogin = true; // ?????/???????
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"????CSV???????{e.Message}");
        }
    }

    /// <summary>
    /// ??CSV???��??????????????????
    /// </summary>
    public void WriteCSVLine(string[] dataArray, bool isAppend = true)
    {
        if (dataArray == null || dataArray.Length == 0)
        {
            Debug.LogError("��??CSV?????????��??????");
            return;
        }

        if (string.IsNullOrWhiteSpace(UserFlieName))
        {
            Debug.LogError("?????????CSV???��???????????");
            return;
        }

        string csvPath = GetCSVFilePath(UserFlieName);
        if (string.IsNullOrEmpty(csvPath) || !File.Exists(csvPath))
        {
            Debug.LogError("CSV?????????????��??");
            return;
        }

        string line = string.Join(",", dataArray);

        try
        {
            // ??File.AppendAllText??????????????StreamWriter��???????
            if (isAppend)
            {
                File.AppendAllText(csvPath, line + Environment.NewLine, Encoding.UTF8);
            }
            else
            {
                File.WriteAllText(csvPath, line + Environment.NewLine, Encoding.UTF8);
            }
            Debug.Log($"CSV????��??????{line}");
        }
        catch (Exception e)
        {
            Debug.LogError($"CSV��??????{e.Message}");
        }
    }
}