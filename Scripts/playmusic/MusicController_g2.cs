using Pico.Platform;
using UnityEngine;
using UnityEngine.UI;
using PXR_Audio.Spatializer;
using DG.Tweening;
using System.Collections;
using TMPro;
using System.Linq;
 
public class MusicController_g2 : MonoBehaviour
{
    public LevelToMusic[] levelMusics; // ��Inspector�����������ļ�
    [SerializeField] private AudioSource p_AudioSource;
    private bool musicEnded = false;
    public GameObject endUI;
    public Transform hudUI;

    public GameObject RightControl;
    public GameObject LeftControl;
    public GameObject RightPause;
    public GameObject LeftPause;

    public bool isLoop;

    public float Maxtime = 300;
    private float startTime;

    void Start()
    {

        if (isLoop)
        {
            startTime = Time.unscaledTime;
            p_AudioSource.loop = true;
        }
    }

    void Update()
    {
        // ������ֲ��Ž��������0.1�룩
        if (!isLoop && !musicEnded && p_AudioSource.time >= p_AudioSource.clip.length - 0.1f)
        {
            OnMusicEnd();
        }

        if (isLoop && !musicEnded && Time.unscaledTime - startTime >= Maxtime)
        {
            OnMusicEnd();
        }
    }

    // ��ʼ�������֣���׼����ɺ���ã�
    public void PlayMusic()
    {
        if (AttentionStats_g2.Instance != null)
        {
            AttentionStats_g2.Instance.ResetStats();
        }

        if (p_AudioSource.isPlaying)
        {
            p_AudioSource.Stop();
        }
        p_AudioSource.clip = levelMusics.First(v => v.levelName == ChartLoader_g2.levelName).c;
        if (p_AudioSource.clip != null)
            p_AudioSource.Play();
        musicStartTime= Time.time;
    }
    private float musicStartTime;
    public float MusicTime{get=>Time.time - musicStartTime;}
    public float MusicLeftTime{get=>p_AudioSource.clip.length-MusicTime;}
    // ���ֽ���ʱ�Ĵ���
    public void OnMusicEnd()
    {
        musicEnded = true;

        //Time.timeScale = 0; // ��ͣ��Ϸ
        PicoUIController.paused = true;
        PicoUIController.Gameover = true;

        p_AudioSource.Stop();

        Vector3 targetpos = new Vector3(endUI.gameObject.transform.position.x, endUI.gameObject.transform.position.y, endUI.gameObject.transform.position.z - 0.1f);

        Text txt = hudUI.GetChild(0).GetChild(2).GetComponent<Text>();

        txt.text = "Max Combo:" + Calculate.maxCombo;
        // �ƶ�λ��
        hudUI.DOMove(targetpos, 1f).SetEase(Ease.InOutQuad);

        hudUI.DOScale(new Vector3(0.004f, 0.004f, 0.004f), 1f).SetEase(Ease.InOutQuad);

        // ��ת��ŷ���� -> Quaternion��
        hudUI.DORotate(new Vector3(0f, 0f, 0f), 1f, RotateMode.Fast).SetEase(Ease.InOutQuad)
            .OnComplete(() => endUI.SetActive(true));

        RightPause.SetActive(true);
        LeftPause.SetActive(true);
        RightControl.SetActive(false);
        LeftControl.SetActive(false);

        if (GetComponent<ChartLoader_g2>())
        {
            GetComponent<ChartLoader_g2>().StopAllCoroutines();
        }

        if (AttentionStats_g2.Instance != null)
        {
          //  AttentionStats_g2.Instance.SavetoCSV();
        }
    }
}