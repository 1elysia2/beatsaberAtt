using UnityEngine;
using UnityEngine.Events;

public class HoldNote : MonoBehaviour
{
    public JudgmentSystem judgmentSystem;
    // ���ò���
    private float holdDuration;      // ��Ҫ��������ʱ��

    // ״̬����
    private bool isActivated = false;     // �Ƿ񱻼����ʼ���Ӵ���
    public bool isHolding = false;       // ��ǰ�Ƿ�����Ӵ���
    private float startTime;      // �״νӴ�ʱ��
    private float durationTime;
    private Collider swordCollider;// �⽣��ײ��
    private float speed;
    public Transform parent;


    // �¼�ϵͳ
    //public UnityEvent<Judgment> OnHoldJudged;

    void Update()
    {
        //// �������ڽ���ʱ���Զ��ж�
        //if (Time.time > spawnTime + holdDuration + 0.5f)
        //{
        //    if (!isActivated)
        //    {
        //        // ��δ���Ӵ�����ֱ���� Bad
        //        Judge(Judgment.HoldBad);
        //    }
        //    Destroy(gameObject);
        //}
    }

    // �ⲿ��ʼ�����ã�����ʱ��¼����ʱ�䣩

    public void Initialize(float duration, float sped)
    {
        holdDuration = duration;
        speed = sped;
    }
    public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    // ����������
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Saber")) // ֻ��� Enemy ��
        {
            Debug.Log("trigger");
            if (!isActivated)
            {
                Debug.Log("trigger");
                isActivated = true;
                isHolding = true;
                startTime = Time.time;
            }
        }

        if(other.TryGetComponent<HoldNote>(out var  h))
        {
            if(h.transform.parent.position.z<transform.parent.position.z)
            {
                Destroy(transform.parent.gameObject);
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (isHolding && other.gameObject.layer == LayerMask.NameToLayer("Saber"))
        {

            float initialLength = holdDuration * speed;
            float length = (Time.time - startTime) * speed;
            float deltaLength = Mathf.Clamp(initialLength - length, 0.05f, initialLength);
            //Debug.Log("lenggth" + deltaLength);
            parent.transform.localScale = new Vector3(0.5f, 1f, deltaLength);

            if (Time.time - startTime > holdDuration + 0.5f)
            {
                isHolding = false;
            }
            //Debug.Log("holding"+length);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        if (isActivated && other.gameObject.layer == LayerMask.NameToLayer("Saber"))
        {
            durationTime = Time.time - startTime;
            Debug.Log("time" + durationTime);
            Debug.Log("needtime" + holdDuration);
            isHolding = false;
            Judgment result = judgmentSystem.Judge(
                durationTime, holdDuration
            );

            Destroy(parent.gameObject);
            Time.timeScale = 1f;

            Calculate.OnHit(result);
        }
    }

    // �ж��߼�����
    //private void Judge(Judgment result)
    //{
    //    // ȷ��ֻ�ж�һ��
    //    if (!isActivated) return;

    //    // ����ж���Ĵ���
    //    isActivated = false;
    //    OnHoldJudged?.Invoke(result);
    //    Destroy(gameObject, 0.1f); // �ӳ������Բ�����Ч
    //}

    // ��������Ƿ񱣳ֵ�����
    //void FixedUpdate()
    //{
    //    if (isHolding && Time.time > startTime + holdDuration)
    //    {
    //        Judge(Judgment.HoldPerfect);
    //    }
    //}
}

