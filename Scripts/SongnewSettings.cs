using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SongnewSettings : MonoBehaviour
{
    public static SongnewSettings Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public PicoUIController UIController;

    [Header("ΩÃ—ß…Ë÷√")]
    public List<PlayableDirector> playables;


    private int currentplayableCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (UIController == null) return;
        currentplayableCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentplayableCount >= playables.Count)
        {
            Destroy(this);
        }
    }

    public void PlayabletoPlay(int index)
    {
        playables[index].Play();
        currentplayableCount++;
    }
}
