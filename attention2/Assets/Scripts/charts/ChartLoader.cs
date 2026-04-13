using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartLoader : MonoBehaviour
{
    public enum GameMode
    {
        Json, Easy, Hard,
    }
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

    public static GameMode mode;
    // 当前游戏模式（Json / Easy / Hard）
    // 静态：用于跨场景传递模式

    public static string levelName;
    // 当前关卡名称（用于加载 Resources/Charts/ 下的谱面）


    // ===============================
    // 谱面文件相关
    // ===============================

    private string chartFileName;
    // 实际加载的 JSON 文件路径（Resources路径）


    private ChartData loadedChart;
    // 解析后的谱面数据（包含所有Note信息）


    private float startTime;
    // 游戏开始时间（用于时间轴计算）


    // ===============================
    //全局时间控制
    // ===============================

    [Range(1, 5)]
    public float timeScale = 1;
    // Unity全局时间缩放倍率
    // 1 = 正常速度
    // >1 = 加速
    // 会影响所有动画、物理和协程


    public Transform[] laneSpawnPoints;// 音符生成轨道（左右轨道） 数组长度 = 轨道数量


    public GameObject notePrefab; // 普通Tap音符预制体

    public GameObject holdNotePrefab; // Hold长按音符预制体

    public GameObject errorPrefab; // 干扰音符预制体（错误刺激）


    private float currentSpawnTime;// 当前生成间隔（会被动态难度修改）

    public float standardspawntime = 4f; // 简单模式默认生成间隔

    public float hardSpawnTime = 1.5f; // 困难模式默认生成间隔

    private float moveSpeedBase = 5f; // 默认基础移动速度

    private float moveSpeedCurrent; // 当前移动速度（会被动态难度修改）

    public float intervalTime = 50f;// 每隔多少秒进行一次节奏微调（动态难度刷新间隔）

    private float lastExecuteTime = 0;
    // 上次执行节奏调整的时间

    private float ExtraTime = 0;
    // 时间补偿量（用于微调音符生成时间）
    // 会随着注意力状态变化逐渐增加

    public MusicController playmusic;
    // 音乐控制器（用于获取剩余播放时间等）

    //初始化难度参数
    void Start()
    { 
        moveSpeedCurrent = moveSpeedBase;//当前音符移动速度

        currentSpawnTime = SelectDefaultSpawnTime();//当前音符时间间隔
    }

    //调整游戏速度
    void Update()
    {
        Time.timeScale = timeScale;
    }

    void OnEnable()
    {
        MusicController musicController = GetComponent<MusicController>();
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

        //初始化注意力报告，实时记录
        AttentionReport.CreateReport(levelName, (int)mode);

        //60秒调整协程
        StartCoroutine(FrequencyChangeCor());

        //判断游戏模式，困难简单读谱
        if (mode == GameMode.Easy || mode == GameMode.Hard)
        {
            StartCoroutine(SpawnNotesRandom(mode == GameMode.Easy));
            return;
        }

        if (string.IsNullOrEmpty(levelName))
            levelName = "16hz-soft-md";
        chartFileName = "Charts/" + levelName;
        TextAsset jsonFile = Resources.Load<TextAsset>(chartFileName);
        loadedChart = JsonUtility.FromJson<ChartData>(jsonFile.text);
        StartCoroutine(SpawnNotesCoroutine());
    }

    //按照谱面精准生成乐谱
    IEnumerator SpawnNotesCoroutine()
    {
        //遍历所有的note
        foreach (NoteData note in loadedChart.notes)
        {
            //如果暂停就停止
            while (PicoUIController.paused)
            {
                yield return null;
            }

            if ((Time.time - lastExecuteTime >= intervalTime))
            {
                ExtraTime += AttentionStats.Instance.Sometime();
                lastExecuteTime = Time.time;
            }

            //让音符提前生成并抵达判定线
            float spawnTime = note.time - CalculateTravelTime() + loadedChart.offset + ExtraTime;

            while (Time.time - startTime < spawnTime)
            {
                yield return null;
            }

            // 生成的音符类型
            GameObject prefab = null;
            if (note.noteType == "hold")
                prefab = holdNotePrefab;
            else if (note.noteType == "tap")
                prefab = notePrefab;
            else if (note.noteType == "error")
                prefab = errorPrefab;

            if (prefab == null)
                continue;
            Transform spawnPoint = laneSpawnPoints[note.lane];
            GameObject newNote = Instantiate(prefab, new Vector3(spawnPoint.position.x, 1.2f, spawnPoint.position.z), spawnPoint.rotation);

            if (note.noteType == "hold")
            {
                // Hold初始化
                HoldNoteVisual holdVisual = newNote.GetComponentInChildren<HoldNoteVisual>();
                holdVisual.Initialize(
                    duration: note.duration,
                    spawnY: spawnPoint.position.y,
                    targetY: transform.position.y, // �ж���λ��
                    speed: 5f
                );
            }
            // normal初始化
            else
            {
                Note noteScript = newNote.GetComponent<Note>();
                noteScript.Initialize(note.direction, moveSpeedCurrent);
            }
        }
    }

    //计算音符到判定线需要的时间
    float CalculateTravelTime()
    {
        float distance = Vector3.Distance(laneSpawnPoints[0].position, transform.position);
        float speed = 5f;
        return distance / speed;
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

            Note noteScript1 = newNote.GetComponent<Note>();
            noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent);
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
                //spawn2的值
                //0表示普通方块
                //1表示hold方块
                int spawn2 = 0;       
                spawn2 = Random.Range(0, 2);

                Transform spawnPoint = Random.value > 0.5f ? laneSpawnPoints[0] : laneSpawnPoints[1];

                if (spawn2 == 0)
                {
                    GameObject newNote = Instantiate(notePrefab, new Vector3(spawnPoint.position.x, 1.2f, spawnPoint.position.z), spawnPoint.rotation);
                    int direction = 0;
                    direction = Random.Range(0, 4);
                    string[] diretionstrings = new string[] { "Up", "Down", "Left", "Right" };

                    Note noteScript1 = newNote.GetComponent<Note>();
                    noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent);
                }
                else
                {
                    GameObject newNote0 = Instantiate(holdNotePrefab, new Vector3(spawnPoint.position.x, 1.2f, spawnPoint.position.z), spawnPoint.rotation);
                    HoldNoteVisual holdVisual = newNote0.GetComponentInChildren<HoldNoteVisual>();

                    float holdduration = Random.Range(0.5f, 2.2f);

                    holdVisual.Initialize(
                        duration: holdduration,
                        spawnY: spawnPoint.position.y,
                        targetY: transform.position.y,
                        speed: moveSpeedCurrent
                    );
                }
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

                Note noteScript1 = newNote1.GetComponent<Note>();
                noteScript1.Initialize(diretionstrings[direction], moveSpeedCurrent);

                //两边同纵向
                if (direction <= 1) 
                {
                    Note noteScript2 = newNote2.GetComponent<Note>();
                    noteScript2.Initialize(diretionstrings[direction], moveSpeedCurrent);
                }
                //两边横向，镜像设计（左右和右左）
                else if (direction == 2) 
                {
                    Note noteScript2 = newNote2.GetComponent<Note>();
                    noteScript2.Initialize(diretionstrings[3], moveSpeedCurrent);
                }
                else 
                {
                    Note noteScript2 = newNote2.GetComponent<Note>();
                    noteScript2.Initialize(diretionstrings[2], moveSpeedCurrent);
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
                var s = AttentionReport.DprimeScore();

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
                       ratingStr= ("注意力不集中");
                    }
                }
                else
                {
                   ratingStr= ("注意力集中");
                    SetFrequency(2);
                }

                AttentionReport.AddReportStamp(ratingStr);
                AttentionStats.Instance.SavetoCSV(FrequencyStr(),ratingStr);
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
        return AttentionReport.HitRate() > 0.55f;
    }

    private bool MissRateHigh()
    {
        return AttentionReport.MissRate() > 0.30f;
    }

    //随机生成音符类型
    IEnumerator SpawnNotesRandom(bool easy)
    {
        while (true)
        {
            while (PicoUIController.paused)      //暂停菜单
            {
                yield return null;
            }

            if (Time.time - lastExecuteTime >= intervalTime)
            {
                ExtraTime += AttentionStats.Instance.Sometime();
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
        if(frequencyMode == mode) return;
        frequencyMode = mode;
        switch (mode)
        {
            case 0:
                currentSpawnTime = SelectDefaultSpawnTime() + 2f;
                moveSpeedCurrent = moveSpeedBase - 1f;
                break;
            case 1:
                currentSpawnTime = SelectDefaultSpawnTime();
                moveSpeedCurrent = moveSpeedBase;
                break;
            case 2:
                currentSpawnTime = SelectDefaultSpawnTime() - 1f;
                moveSpeedCurrent = moveSpeedBase + 1f;
                break;
        }
        foreach (var v in FindObjectsOfType<Note>())
        {
            v.ChangeSpeed(moveSpeedCurrent);
        }

        foreach (var v in FindObjectsOfType<HoldNote>())
        {
            v.ChangeSpeed(moveSpeedCurrent);
        }


    }
    private float SelectDefaultSpawnTime()
    {
        return mode == GameMode.Easy ? standardspawntime : hardSpawnTime;
    }
}