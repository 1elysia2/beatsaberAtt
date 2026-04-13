using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aftercutdestory : MonoBehaviour
{
    [Tooltip("���������ֵС��0������")]
    public float Destorytime = 1f;

    private void OnEnable()
    {
        if(Destorytime < 0f)
        {
            return;
        }

        Destroy(gameObject,Destorytime);
    }
}
