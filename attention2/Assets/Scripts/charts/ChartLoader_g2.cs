using System.Collections;
using System.Collections.Generic;
using Pico.Platform.Models;
using UnityEngine;

public class ChartLoader_g2 : MonoBehaviour
{

    //public static GameMode mode;
    //public static string levelName;
    //private string chartFileName;
    //[Range(1, 5)]
    //public float timeScale = 1; 
    //public Transform[] laneSpawnPoints;//轨道
    //public GameObject notePrefab, errorPrefab;
    //public MusicController playmusic;

    //private ChartData loadedChart;
    //private float startTime; //开始记录时间

    //public GameObject holdNotePrefab; //Hold����Ԥ����

    //public float intervalTime = 50f;    //ÿ������ʱ���޸�һ�������ٶ�
    //private float lastExecuteTime = 0;      //�ϴ��޸ļ�¼��ʱ��
    //private float ExtraTime = 0;        //�����޸ĵ�ʱ�

    //public float standardspawntime = 3f;//简单模式预设生成间隔速度
    //public float hardSpawnTime = 1.5f;//困难模式预设生成间隔
    //private float currentSpawnTime;
    //private float moveSpeedBase = 5f;
    //private float moveSpeedCurrent;

    public static bool tutorial = false;
    public static string levelName = "16hz-hard-hd";
    // 当前关卡名称（用于加载 Resources/Charts/ 下的谱面）


    // ===============================
    // 谱面文件相关
    // ===============================

    private string chartFileName;
    // 实际加载的 JSON 文件路径（Resources路径）

    public GameObject[] tutorialTips;
    private ChartData loadedChart;
    // 解析后的谱面数据（包含所有Note信息）


    private float startTime;
    // 游戏开始时间（用于时间轴计算）


    // ===============================
    //全局时间控制
    // ===============================


    public Transform[] laneSpawnPoints;// 音符生成轨道（左右轨道） 数组长度 = 轨道数量


    public GameObject notePrefab; // 普通Tap音符预制体 

    public GameObject errorPrefab; // 干扰音符预制体（错误刺激）


    private float currentSpawnTime;// 当前生成间隔（会被动态难度修改）

    public float standardspawntime = 4f; // 简单模式默认生成间隔

    private float moveSpeedBase = 0.3f; // 默认基础移动速度

    private float moveSpeedCurrent; // 当前移动速度（会被动态难度修改）

    public float intervalTime = 50f;// 每隔多少秒进行一次节奏微调（动态难度刷新间隔）

    private float lastExecuteTime = 0;
    // 上次执行节奏调整的时间

    private float ExtraTime = 0;
    // 时间补偿量（用于微调音符生成时间）
    // 会随着注意力状态变化逐渐增加

    public MusicController_g2 playmusic;
    // 音乐控制器（用于获取剩余播放时间等）

    //初始化难度参数
    void Start()
    {
        moveSpeedCurrent = moveSpeedBase;//当前音符移动速度

        currentSpawnTime = standardspawntime;//当前音符时间间隔
    }


    void OnEnable()
    {
        MusicController_g2 musicController = GetComponent<MusicController_g2>();
        if (musicController != null)
        {
            //播放音乐
            musicController.PlayMusic();
        }
        else
        {
            Debug.LogError("MusicController δ�ҵ�����������Ƿ�������");
        }
        //记录游戏开始时间
        startTime = Time.time; // ��¼��Ϸ����ʱ��
        lastExecuteTime = startTime;
        if (tutorial)
        {
            StartCoroutine(TutorialCor());
        }
        else
        {
            //初始化注意力报告，实时记录
            AttentionReport_g2.CreateReport(levelName);

            //60秒调整协程
            StartCoroutine(FrequencyChangeCor());
            StartCoroutine(SpawnNotesRandom());
        }


    }


    //计算音符到判定线需要的时间
    float CalculateTravelTime()
    {
        return 2f;
    }

    //随机模式代码
    void SpawnRandom()
    {
        //错误音符占比12.5%
        bool spawnError = Random.Range(0, 1f) < 0.125f;

        if (spawnError)
        {
            Transform spawnPoint = Random.value > 0.5f ? laneSpawnPoints[0] : laneSpawnPoints[1];
            //随机生成方块
            GameObject newNote = Instantiate(errorPrefab, new Vector3(spawnPoint.position.x, 1.2f, spawnPoint.position.z), spawnPoint.rotation);
            int direction = 0;
            direction = Random.Range(0, 4);
            string[] diretionstrings = new string[] { "Up", "Down", "Left", "Right" };

            Note_g2 noteScript1 = newNote.GetComponent<Note_g2>();

            noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent, spawnPoint == laneSpawnPoints[0] ? 1 : -1);
        }
        else
        //进入常规方块生成
        {
            int spawn1 = 1;
            spawn1 = Random.Range(1, 3);

            //spawn1的值
            //1表示单轨道生成
            //2表示双轨道生成
            if (spawn1 == 1)
            {
                Transform spawnPoint = Random.value > 0.5f ? laneSpawnPoints[0] : laneSpawnPoints[1];
                GameObject newNote = Instantiate(notePrefab, new Vector3(spawnPoint.position.x, 1.2f, spawnPoint.position.z), spawnPoint.rotation);
                int direction = 0;
                direction = Random.Range(0, 4);
                string[] diretionstrings = new string[] { "Up", "Down", "Left", "Right" };

                Note_g2 noteScript1 = newNote.GetComponent<Note_g2>();
                noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent, spawnPoint == laneSpawnPoints[0] ? 1 : -1);


            }
            //双轨道
            else
            {
                //左轨和右轨各初始化一个方块
                GameObject newNote1 = Instantiate(notePrefab, new Vector3(laneSpawnPoints[0].position.x, 1.2f, laneSpawnPoints[0].position.z), laneSpawnPoints[0].rotation);
                GameObject newNote2 = Instantiate(notePrefab, new Vector3(laneSpawnPoints[1].position.x, 1.2f, laneSpawnPoints[1].position.z), laneSpawnPoints[1].rotation);

                int direction = 0;
                direction = Random.Range(0, 4);
                string[] diretionstrings = new string[] { "Up", "Down", "Left", "Right" };

                Note_g2 noteScript1 = newNote1.GetComponent<Note_g2>();
                noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent, 1);

                //两边同纵向
                if (direction <= 1)
                {
                    Note_g2 noteScript2 = newNote2.GetComponent<Note_g2>();
                    noteScript2.Initialize(diretionstrings[direction], moveSpeedCurrent, -1);
                }
                //两边横向，镜像设计（左右和右左）
                else if (direction == 2)
                {
                    Note_g2 noteScript2 = newNote2.GetComponent<Note_g2>();
                    noteScript2.Initialize(diretionstrings[3], moveSpeedCurrent, -1);
                }
                else
                {
                    Note_g2 noteScript2 = newNote2.GetComponent<Note_g2>();
                    noteScript2.Initialize(diretionstrings[2], moveSpeedCurrent, -1);
                }
            }
        }
    }

    //难度调节系统
    IEnumerator FrequencyChangeCor()
    {

        float timer = 0;

        while (true)
        {
            while (PicoUIController.paused)
            {
                yield return null;
            }

            timer += Time.deltaTime;
            yield return 0;
            //60秒评估一次注意力D-prime
            if (timer >= 60)
            {
                timer -= 60;
                var s = AttentionReport_g2.DprimeScore();

                string ratingStr = "无";
                if (s <= 1)
                {
                    if (HitRateHigh() && MissRateHigh())
                    {
                        SetFrequency(0);
                        ratingStr = "太难了";

                    }
                    else if (!HitRateHigh() && !MissRateHigh())
                    {
                        SetFrequency(2);
                        ratingStr = ("注意力不集中");
                    }
                }
                else
                {
                    ratingStr = ("注意力集中");
                    SetFrequency(2);
                }

                AttentionReport_g2.AddReportStamp(ratingStr);
                AttentionStats_g2.Instance.SavetoCSV(FrequencyStr(), ratingStr);
            }

        }

    }

    //三种动态难度模式，根据模式修改音符产生难度
    private string FrequencyStr()
    {
        if (frequencyMode == 0)
            return "慢";
        if (frequencyMode == 1)
            return "中";
        if (frequencyMode == 2)
            return "快";
        return "";
    }

    //动态注意力调整指标，击中
    private bool HitRateHigh()
    {
        return AttentionReport_g2.HitRate() > 0.55f;
    }

    private bool MissRateHigh()
    {
        return AttentionReport_g2.MissRate() > 0.30f;
    }
    IEnumerator TutorialCor()
    {
        yield return new WaitForSeconds(3);

        Transform spawnPoint = Random.value > 0.5f ? laneSpawnPoints[0] : laneSpawnPoints[1];
        GameObject newNote = Instantiate(notePrefab, new Vector3(spawnPoint.position.x, 1.2f, spawnPoint.position.z), spawnPoint.rotation);
        int direction = 0;
        direction = Random.Range(0, 4);
        string[] diretionstrings = new string[] { "Up", "Down", "Left", "Right" };

        Note_g2 noteScript1 = newNote.GetComponent<Note_g2>();
        noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent, spawnPoint == laneSpawnPoints[0] ? 1 : -1);
        (noteScript1.movement as NoteMovement_g2).EnterTop.AddListener(() =>
        {
            Time.timeScale = 0.2f;
            tutorialTips[0].SetActive(true);
        });
        (noteScript1.movement as NoteMovement_g2).ExitTop.AddListener(() =>
     {
         Time.timeScale = 1f;
         tutorialTips[0].SetActive(false);
     });

        yield return new WaitForSeconds(2);


        spawnPoint = Random.value > 0.5f ? laneSpawnPoints[0] : laneSpawnPoints[1];

        newNote = Instantiate(errorPrefab, new Vector3(spawnPoint.position.x, 1.2f, spawnPoint.position.z), spawnPoint.rotation);

        direction = Random.Range(0, 4);
        diretionstrings = new string[] { "Up", "Down", "Left", "Right" };

        noteScript1 = newNote.GetComponent<Note_g2>();

        noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent, spawnPoint == laneSpawnPoints[0] ? 1 : -1);


        (noteScript1.movement as NoteMovement_g2).EnterTop.AddListener(() =>
        {
            Time.timeScale = 0.2f;
            tutorialTips[1].SetActive(true);
        });
        (noteScript1.movement as NoteMovement_g2).ExitTop.AddListener(() =>
     {
         Time.timeScale = 1f;
         tutorialTips[1].SetActive(false);
     });
        yield return new WaitForSeconds(2);


        //左轨和右轨各初始化一个方块
        GameObject newNote1 = Instantiate(notePrefab, new Vector3(laneSpawnPoints[0].position.x, 1.2f, laneSpawnPoints[0].position.z), laneSpawnPoints[0].rotation);
        GameObject newNote2 = Instantiate(errorPrefab, new Vector3(laneSpawnPoints[1].position.x, 1.2f, laneSpawnPoints[1].position.z), laneSpawnPoints[1].rotation);

        direction = 0;
        direction = Random.Range(0, 4);
        diretionstrings = new string[] { "Up", "Down", "Left", "Right" };

        noteScript1 = newNote1.GetComponent<Note_g2>();
        noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent, 1);

        //两边同纵向
        if (direction <= 1)
        {
            Note_g2 noteScript2 = newNote2.GetComponent<Note_g2>();
            noteScript2.Initialize(diretionstrings[direction], moveSpeedCurrent, -1);
        }
        //两边横向，镜像设计（左右和右左）
        else if (direction == 2)
        {
            Note_g2 noteScript2 = newNote2.GetComponent<Note_g2>();
            noteScript2.Initialize(diretionstrings[3], moveSpeedCurrent, -1);
        }
        else
        {
            Note_g2 noteScript2 = newNote2.GetComponent<Note_g2>();
            noteScript2.Initialize(diretionstrings[2], moveSpeedCurrent, -1);
        }

        (noteScript1.movement as NoteMovement_g2).EnterTop.AddListener(() =>
    {
        Time.timeScale = 0.2f;
        tutorialTips[2].SetActive(true);
    });
        (noteScript1.movement as NoteMovement_g2).ExitTop.AddListener(() =>
     {
         Time.timeScale = 1f;
         tutorialTips[2].SetActive(false);
     });

        yield return new WaitForSeconds(2);

        Time.timeScale = 1;


        float startTime = Time.time;

        while (Time.time- startTime <60)
        {
            while (PicoUIController.paused)      //暂停菜单
            {
                yield return null;
            } 
            if (Time.time - lastExecuteTime >= intervalTime)
            {
                ExtraTime += AttentionStats_g2.Instance.Sometime();
                Debug.Log(ExtraTime);
                lastExecuteTime = Time.time;
            }
            float waittime = currentSpawnTime - CalculateTravelTime() + ExtraTime;
            waittime = Mathf.Max(0.5f, waittime);

            yield return new WaitForSecondsRealtime(waittime); 
            SpawnRandom(); 
        }

        yield return new WaitForSeconds(1);

       GetComponent< MusicController_g2>().OnMusicEnd();
  

    }
    //随机生成音符类型
    IEnumerator SpawnNotesRandom()
    {
        while (true)
        {
            while (PicoUIController.paused)      //暂停菜单
            {
                yield return null;
            }

            if (Time.time - lastExecuteTime >= intervalTime)
            {
                ExtraTime += AttentionStats_g2.Instance.Sometime();
                Debug.Log(ExtraTime);
                lastExecuteTime = Time.time;
            }
            float waittime = currentSpawnTime - CalculateTravelTime() + ExtraTime;
            waittime = Mathf.Max(0.5f, waittime);

            yield return new WaitForSecondsRealtime(waittime);


            if (playmusic.MusicLeftTime > CalculateTravelTime())
                SpawnRandom();


        }
    }
    private int frequencyMode = 1;

    //修改音符产生难度
    private void SetFrequency(int mode)
    {
        if (frequencyMode == mode) return;
        frequencyMode = mode;
        switch (mode)
        {
            case 0:
                currentSpawnTime = standardspawntime + 2f;
                //   moveSpeedCurrent = moveSpeedBase - 1f;
                break;
            case 1:
                currentSpawnTime = standardspawntime;
                //    moveSpeedCurrent = moveSpeedBase;
                break;
            case 2:
                currentSpawnTime = standardspawntime - 1f;
                //  moveSpeedCurrent = moveSpeedBase + 1f;
                break;
        }
        // foreach (var v in FindObjectsOfType<Note>())
        // {
        //     v.ChangeSpeed(moveSpeedCurrent);
        // }

        // foreach (var v in FindObjectsOfType<HoldNote>())
        // {
        //     v.ChangeSpeed(moveSpeedCurrent);
        // }


    }
}