using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{

    private Slider  s;

    void Awake()
    {
         s =GetComponent<Slider>();
    }
    void OnEnable()
    {
        s.SetValueWithoutNotify(VolumeSet.volume);
    }

    void Update()
    {
        VolumeSet.volume= s.value;
    }

}
