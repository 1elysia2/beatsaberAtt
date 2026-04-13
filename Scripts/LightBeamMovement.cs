using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamMovement : MonoBehaviour
{
    public float z1,z2;

    private float euler;
    public float speed = 50;
    void Awake()
    {
        euler =transform.eulerAngles .z;

    }
    void Update()
    {
        var pingpong= Mathf.PingPong(euler/speed+Time.time,1);

        var z = Mathf.Lerp(z1,z2,pingpong);

        transform.eulerAngles = new Vector3(0,0,z);
    }
}
