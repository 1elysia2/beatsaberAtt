using UnityEngine;

public class SaberCollision : MonoBehaviour
{
    public JudgmentSystem judgmentSystem;
    public LayerMask noteLayer;
    private Vector3 inPos;
    private Vector3 outPos;
    private float lastTime;
    private Vector3 lastPosition;
    private TrailRenderer trail;

    void Awake()
    {
        judgmentSystem = FindAnyObjectByType<JudgmentSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & noteLayer) != 0)
        {
            inPos = transform.position;
            lastTime = Time.time;
            Debug.Log("in" + inPos);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        outPos = transform.position;
        Debug.Log("out" + outPos);
        Note note = other.GetComponent<Note>();

        if (note != null)
        {
            Judgment result = judgmentSystem.Judge(
                note,
                other.transform.position,
                Time.time - lastTime,
                outPos - inPos
            );

            //�����и�����ɶ���������ʾ������Ч
            if (AfterCutManager.Instance != null)
            {
                AfterCutManager.Instance.SwanperCut(result, other.transform, note.noteDirection);
            }

            if (note is Note_g2)
            {
                AttentionEventHub.Instance.OnNoteHit(result,Time.time -( note as Note_g2).spawnTime);
            }
            else
            {
                AttentionEventHub.Instance.OnNoteHit(result);
            }
            Time.timeScale = 1.0f;
            Calculate.OnHit(result); 
            Destroy(other.gameObject);
        }
    }
}
