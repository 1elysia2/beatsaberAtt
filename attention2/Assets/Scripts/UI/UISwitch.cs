using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitch : MonoBehaviour
{
    public GameObject Begin;
    public GameObject Choose;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openChoose()
    {
        // Begin.SetActive(false);
        // Choose.SetActive(true);
    }

    public void closeChoose()
    {
      //  Choose.SetActive(false);
      //  Begin.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        // �� Unity �༭����ֹͣ������Ϸ
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // �ڴ������Ϸ���˳���Ϸ
        Application.Quit();
#endif
    }
}
