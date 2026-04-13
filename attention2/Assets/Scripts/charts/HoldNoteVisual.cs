using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNoteVisual : MonoBehaviour
{
    public float duration;
    private float startY;
    private float targetY;
    public float speed;
    private float startTime;
    private float initialLength;
    public HoldNote note;
    public Transform parent;
    private GameObject tipUI2;
    public GameObject rateUI;

    public void Initialize(float duration, float spawnY, float targetY, float speed)
    {
        this.duration = duration;
        this.startY = spawnY;
        this.targetY = targetY;
        this.speed = speed;
        this.startTime = Time.time;

        // 根据持续时间计算初始长度
        initialLength = speed * duration;
        parent.transform.localScale = new Vector3(0.5f, 0.8f, initialLength);
        note.Initialize(duration,speed);
    }

    private void Start()
    {
        tipUI2 = GameObject.FindGameObjectWithTag("tip2");
        if (rateUI != null)
        {
            rateUI.SetActive(true);
        }
        if (tipUI2 != null)
        {
            tipUI2.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!PicoUIController.paused&&!note.isHolding)
        {   // 计算移动方向（Z 轴正方向或负方向）
            Vector3 direction =  Vector3.back;

            // 沿 Z 轴移动
            parent.transform.Translate(direction * speed * Time.deltaTime);

            if (tipUI2 != null && parent.transform.position.z < 8.0f && parent.transform.position.z > 5.0f)
            {
                // 激活 tipUI2 并隐藏 rateUI
                //if (rateUI != null)
                //{
                //    rateUI.SetActive(false);
                //}
                if(!tipUI2.transform.GetChild(0).gameObject.activeSelf)
                tipUI2.transform.GetChild(0).gameObject.SetActive(true);

                Time.timeScale = 0.1f;
            }else if (tipUI2 != null && parent.transform.position.z < 5.0f)
            {
                //if (rateUI != null)
                //{
                //    rateUI.SetActive(false);
                //}
                tipUI2.transform.GetChild(0).gameObject.SetActive(true);
                Time.timeScale = 0.7f;
            }

            if (parent.transform.position.z < -initialLength+2)
            {
                Calculate.notHit(); 
          
                Destroy(parent.gameObject);
            }
        }

        if(PicoUIController.Gameover)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        if (tipUI2 != null)
        {
            tipUI2.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (rateUI != null)
        {
            rateUI.SetActive(true);
        }
    }
}
