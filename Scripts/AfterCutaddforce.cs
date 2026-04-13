using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//物体切割特效
public class AfterCutaddforce : MonoBehaviour
{
    [Header("推力设置")]
    [Tooltip("施加的力大小（向上）")]
    public float forceMagnitude = 10f;

    [Tooltip("力的模式：Impulse = 瞬间冲量，Force = 持续力")]
    public ForceMode forceMode = ForceMode.Impulse;

    [Tooltip("是否向相反方向施加力")]
    public bool isAddtotheContrary = false;

    [Tooltip("是否使用世界坐标系的Y轴（勾选=true），不勾选=使用物体自身的本地Y轴")]
    public bool useWorldUp = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 安全检查
        if (rb == null)
        {
            Debug.LogError("物体上没有找到 Rigidbody 组件！", this);
            enabled = false;
        }
    }

    void OnEnable()
    {
        if (rb == null) return;

        Vector3 direction;

        if (useWorldUp)
        {
            if (isAddtotheContrary)
            {
                direction = -Vector3.up;
            }
            else
            {
                // 沿世界 Y 轴向上（最常见情况，比如跳跃）
                direction = Vector3.up;
            }
        }
        else
        {
            if (isAddtotheContrary)
            {
                direction = -transform.up;
            }
            else
            {
                // 沿物体自身本地 Y 轴方向（比如角色朝向天空的方向）
                direction = transform.up;
            }
        }

        // 施加力
        rb.AddForce(direction * forceMagnitude, forceMode);
    }
}
