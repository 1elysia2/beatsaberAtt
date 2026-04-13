using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance;

    public AudioClip defaultHitSound;
    public AudioSource audioSourceTemplate; // 拖入预配置的 AudioSource

    private List<AudioSource> audioSourcePool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化音效池（基于模板 AudioSource）
        audioSourcePool = new List<AudioSource>();
        for (int i = 0; i < 5; i++) // 池大小可调整
        {
            AudioSource newSource = Instantiate(audioSourceTemplate, transform);
            newSource.gameObject.SetActive(false); // 初始隐藏
            audioSourcePool.Add(newSource);
        }
    }

    public void PlayHitSound(AudioClip clip = null)
    {
        AudioClip soundToPlay = clip ?? defaultHitSound;
        if (soundToPlay == null) return;

        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                source.gameObject.SetActive(true);
                source.PlayOneShot(soundToPlay);
                StartCoroutine(DeactivateAfterPlay(source, soundToPlay.length));
                return;
            }
        }

        // 如果池不足，临时创建（备用方案）
        AudioSource tempSource = Instantiate(audioSourceTemplate, transform);
        tempSource.PlayOneShot(soundToPlay);
        Destroy(tempSource.gameObject, soundToPlay.length);
    }

    private IEnumerator DeactivateAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false); // 重置状态
    }
}
