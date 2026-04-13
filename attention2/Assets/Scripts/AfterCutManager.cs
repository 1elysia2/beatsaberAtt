using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterCutManager : MonoBehaviour
{
    private static AfterCutManager _instance;
    public static AfterCutManager Instance => _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("�п���Ԥ����")]
    public GameObject AfterCutprefab,AfterCutErrorPrefab;
    public GameObject effect;

    public void SwanperCut(Judgment judgment, Transform swanpertrf, string notedirection) 
    {
        switch (judgment)
        {
            case Judgment.Bad:
            case Judgment.HoldBad:
                {
                    return;
                }
            
        } 
        GameObject aftercut = Instantiate(judgment == Judgment.ErrorCube?AfterCutErrorPrefab:AfterCutprefab, swanpertrf.position, Quaternion.identity);
        GameObject eff = Instantiate(effect, swanpertrf.position, Quaternion.identity);
        aftercut.transform.SetParent(null);

        float z = 0;
        switch (notedirection)
        {
            case "Up":
                {
                    z = 90;
                    eff.transform.eulerAngles = new Vector3(-90, 0, 0);
                    break;
                }
            case "Down":
                {
                    eff.transform.eulerAngles = new Vector3(90, 0, 0);
                    z = 90;
                    break;
                }
            case "Left":
                {
                    eff.transform.eulerAngles = new Vector3(-180, 90, 90);
                  
                    break;
                }
            case "Right":
                {
                    eff.transform.eulerAngles = new Vector3(180, -90, -90);
                 
                    break;
                }

        }

        aftercut.transform.rotation = Quaternion.Euler(0, 0, z);
    }
}
