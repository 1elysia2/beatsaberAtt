using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathColor : MonoBehaviour
{
    private Material instance;
   [ColorUsage(true, true)]
    public Color c1,c2;

    public float speed= 1;
    void Awake()
    {
        var r = GetComponent<MeshRenderer>();

        instance =Instantiate(r.sharedMaterial);
       r.material =instance;
    }

    void Update()
    {
        instance.SetColor("_EmissionColor",Color.Lerp(c1,c2,Mathf.PingPong(Time.time*speed,1)));
    }

}
