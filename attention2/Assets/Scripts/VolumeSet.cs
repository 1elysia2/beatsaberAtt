using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSet : MonoBehaviour
{
    public static float volume =1;
    private AudioSource s;
    void Start()
    {
        s = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        s.volume = volume;
    }
}
