using UnityEngine;
using UnityEngine.SceneManagement;

public class AttentionEventHub : MonoBehaviour
{
    public static AttentionEventHub Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnNoteHit(Judgment judgment,float reactionTime = 0)
    {
        if (AttentionStats.Instance != null && !SceneManager.GetActiveScene().name.Contains("g2"))
        {
            AttentionStats.Instance.RecordHit(judgment);
        }
        if(AttentionStats_g2.Instance!=null && SceneManager.GetActiveScene().name.Contains("g2"))
        {
            AttentionStats_g2.Instance.RecordHit(judgment,reactionTime);
        }
    }

    public void OnNoteMiss()
    {
       if (AttentionStats.Instance != null && !SceneManager.GetActiveScene().name.Contains("g2"))
        {
            AttentionStats.Instance.RecordMiss();
        }
             if(AttentionStats_g2.Instance!=null && SceneManager.GetActiveScene().name.Contains("g2"))
        {
            AttentionStats_g2.Instance.RecordMiss();
        }
    }
}
