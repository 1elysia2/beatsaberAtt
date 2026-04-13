//using UnityEngine;
//using UnityEngine.UI;

//public class SoundDepth : MonoBehaviour
//{
//    public Button soundButton;  // 音量按钮
//    public Slider volumeSlider; // 音量滑动条
//    public PXR_Audio_Spatializer_AudioListener spatialAudioListener;

//    void Start()
//    {
//        // 初始化时将滑动条隐藏
//        volumeSlider.gameObject.SetActive(false);

//        // 初始化滑动条的值为当前音量
//        volumeSlider.value = spatialAudioListener.volume;

//    }

//    // 切换滑动条显示或隐藏
//    public void ToggleVolumeSlider()
//    {
//        bool isActive = volumeSlider.gameObject.activeSelf;
//        volumeSlider.gameObject.SetActive(!isActive);
//    }

//    // 设置音量
//    public void SetVolume(float volume)
//    {
//        spatialAudioListener.volume = volume;    // 调整音量
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using Unity.XR.PXR;  // 正确命名空间！（删除旧 using PXR）

public class SoundDepth : MonoBehaviour
{
    public Button soundButton;
    public Slider volumeSlider;

    void Start()
    {
        volumeSlider.gameObject.SetActive(false);

        // Editor 测试用 AudioListener，设备用 PXR_System
        UpdateSliderValue();
    }

    void UpdateSliderValue()
    {
        float vol = 0.5f;  // 默认 50%
#if UNITY_ANDROID && PICO_UNITY_INTEGRATION_ENABLED
        int sysVol = PXR_System.GetVolumeNum();
        if (sysVol >= 0) vol = sysVol / 15f;
#endif
        volumeSlider.value = vol;
    }

    public void ToggleVolumeSlider()
    {
        bool isActive = volumeSlider.gameObject.activeSelf;
        volumeSlider.gameObject.SetActive(!isActive);
        if (!isActive) UpdateSliderValue();  // 显示时刷新值
    }

    public void SetVolume(float volume)
    {
#if UNITY_ANDROID && PICO_UNITY_INTEGRATION_ENABLED
        int volInt = Mathf.RoundToInt(Mathf.Clamp01(volume) * 15f);
        PXR_System.SetVolumeNum(volInt);
#else
        AudioListener.volume = volume;  // Editor/其他平台 fallback
#endif
    }
}