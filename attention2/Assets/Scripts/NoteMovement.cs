using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    public float speed = 2f;
    public bool moveTowardsNegativeZ = true; // Ĭ���� Z �Ḻ�����ƶ�
    public Note note;
    private GameObject tipUI1;
    private GameObject tipUI3;
    public GameObject rateUI;
    protected virtual void Start()
    {
        tipUI1 = GameObject.FindGameObjectWithTag("tip1");
        tipUI3 = GameObject.FindGameObjectWithTag("tip3");

        if (rateUI != null)
        {
            rateUI.SetActive(true);
        }
        if (tipUI1 != null)
        {
            tipUI1.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (tipUI3 != null)
        {
            tipUI3.transform.GetChild(0).gameObject.SetActive(false);
        }

        Vector3 dir = Vector3.back * 0.7f; ;
        var c = Physics.OverlapSphere(transform.position + dir, 0.1f);
        if (c != null)
        {
            foreach (var v in c)
            {
                if (v.transform.TryGetComponent<HoldNote>(out var h))
                {
                    Destroy(gameObject);
                    break;
                }
            }
        }

    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HoldNote>(out var h))
        {
            Destroy(gameObject);
        }

    }
    protected virtual void Update()
    {
        if (!PicoUIController.paused)
        {   // �����ƶ�����Z ��������򸺷���
            Vector3 direction = Vector3.back;

            // �� Z ���ƶ�
            transform.Translate(direction * speed * Time.deltaTime);
            if (!note.isError)
            {
                if (tipUI1 != null && transform.position.z < 6.0f)
                {
                    //if (rateUI != null)
                    //{
                    //    rateUI.SetActive(false);
                    //}
                    if (!tipUI1.transform.GetChild(0).gameObject.activeSelf)
                        tipUI1.transform.GetChild(0).gameObject.SetActive(true);

                    Debug.Log("stop");
                    Time.timeScale = 0.1f;
                }
            }
            else
            {
                if (tipUI3 != null && transform.position.z < 6.0f)
                {
                    //if (rateUI != null)
                    //{
                    //    rateUI.SetActive(false);
                    //}
                    if (!tipUI3.transform.GetChild(0).gameObject.activeSelf)
                        tipUI3.transform.GetChild(0).gameObject.SetActive(true);

                    Debug.Log("stop");
                    Time.timeScale = 0.1f;
                }
                if (transform.position.z < 2.3f)
                {
                    tipUI3.transform.GetChild(0).gameObject.SetActive(false);

                    Time.timeScale = 1f;
                }
            }


            if (!note.isError && transform.position.z < 2.3f)
            {
                Calculate.notHit();
                AttentionEventHub.Instance.OnNoteMiss();

                Time.timeScale = 1f;
                if (tipUI1 != null)
                {
                    tipUI1.transform.GetChild(0).gameObject.SetActive(false);
                }
                if (rateUI != null)
                {
                    rateUI.SetActive(true);
                }
                Destroy(gameObject);
            }
        }
        if (PicoUIController.Gameover)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        Time.timeScale = 1f;
        if (tipUI1 != null)
        {
            tipUI1.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (rateUI != null)
        {
            rateUI.SetActive(true);
        }
    }
}

