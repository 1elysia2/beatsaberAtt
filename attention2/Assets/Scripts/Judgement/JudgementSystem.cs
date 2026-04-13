using UnityEngine;

public enum Judgment
{
    Perfect,
    Good,
    Bad,
    HoldPerfect,
    HoldBad,
    ErrorCube,
}

public class JudgmentSystem : MonoBehaviour
{
    public JudgmentSettings settings;
    public Transform judgmentPlane; // 拖拽判定平面到此处

    // 判断是否在有效空间范围内
    private string IsInHitZone(Vector3 notePosition)
    {
        if (judgmentPlane == null)
        {
            return "Good_g2";
        }
        else
        {
            float distanceToPlane = Mathf.Abs(notePosition.z - judgmentPlane.position.z);
            if (distanceToPlane <= 1 && distanceToPlane >= -1)
            {
                return "Perfect";
            }
            else if (distanceToPlane > 1 && distanceToPlane < settings.hitDistance)
            {
                return "Good";
            }
            else
            {
                return "Bad";
            }
        }

    }

    // 综合时间和空间判定
    public Judgment Judge(Note note, Vector3 notePosition, float noteTime, Vector3 speedDir)
    {
        if (note.isError)
            return Judgment.ErrorCube;
        Debug.Log("位移" + speedDir);
        //计算切割方向
        string spDir;
        Vector3 velocity = speedDir / noteTime;

        // 将速度投影到水平面
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(velocity, Vector3.forward).normalized;
        Debug.Log("速度方向: " + projectedVelocity);


        // 使用点积判断主要方向
        float dotForward = Vector3.Dot(projectedVelocity, Vector3.up);
        float dotRight = Vector3.Dot(projectedVelocity, Vector3.right);

        // 判断主导方向
        if (Mathf.Abs(dotForward) > Mathf.Abs(dotRight))
        {
            if (dotForward < 0) spDir = "Down";
            else spDir = "Up";
        }
        else
        {
            if (dotRight < 0) spDir = "Left";
            else spDir = "Right";
        }

        Debug.Log("方向" + spDir);
        //方向是否正确
        if (spDir != note.noteDirection)
        {
            Debug.Log("方向不对");
            return Judgment.Bad;
        }

        int score = 0;

        Debug.Log("离区域距离" + notePosition.z);
        //是否在区域内
        if (IsInHitZone(notePosition) == "Perfect")
        {
            score += 3;
        }
        else if (IsInHitZone(notePosition) == "Good")
        {
            score += 1;
        }
        else if(IsInHitZone(notePosition) == "Good_g2")
        { 
            return Judgment.Good;
        }
        else
        {
            return Judgment.Bad;
        }

        Debug.Log("时间" + noteTime);
        //时间
        if (noteTime <= settings.perfectTimeRange)
        {
            score += 2;
        }
        if (noteTime <= settings.goodTimeRange)
        {
            score += 1;
        }

        if (score > 3)
        {
            return Judgment.Perfect;
        }
        else if (score > 1)
        {
            return Judgment.Good;
        }
        else { return Judgment.Bad; }

    }

    public Judgment Judge(float dutime, float needTime)
    {
        if (Mathf.Abs(dutime - needTime) < 0.3)
        {
            return Judgment.HoldPerfect;
        }
        else
        {
            return Judgment.HoldBad;
        }
    }

}