using UnityEngine;

public class EndUIController : MonoBehaviour
{
    public GameObject reportUI;
    public GameObject Hudui;
    public GameObject reportPanel;

    public void GoToReport()
    {
        reportUI.SetActive(true);
        reportPanel.SetActive(true);

        // 选择一种：
        gameObject.SetActive(false);
        Hudui.SetActive(false);
        // Destroy(gameObject);        
    }

    public void BacktoEnd()
    {
        reportUI.SetActive(false);
        reportPanel.SetActive(false);

        // 选择一种：
        gameObject.SetActive(true);
        Hudui.SetActive(true);
    }
}
