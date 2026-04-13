using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PicoUIController : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject uiCanvas;
    private bool isUIVisible = false;

    public GameObject MaskUI;
    public Text btext;

    public GameObject emitter;

    public AudioSource audioSource;

    public GameObject RightControl;
    public GameObject LeftControl;
    public GameObject RightPause;
    public GameObject LeftPause;

    [Header("Pico Button Binding")]
    private XRNode controllerNodeL = XRNode.LeftHand;
    private XRNode controllerNodeR = XRNode.RightHand;
    // ��Pico�ֱ���A/X��
    private InputHelpers.Button triggerButton = InputHelpers.Button.PrimaryButton;

    [Header("��������")]
    public float debounceTime = 0.2f;
    private float lastTriggerTime = 0f;

    [Header("��ǰ��������")]
    public string currentscenename;

    public static bool paused;
    public static bool Gameover;

    void Start()
    {
        paused = false;
        Gameover = false;
        // ��ʼ��������ʱ��׼���޸��״δ����ӳ٣�
        lastTriggerTime = Time.unscaledTime;

        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
            RightPause.SetActive(false);
            LeftPause.SetActive(false);
            RightControl.SetActive(true);
            LeftControl.SetActive(true);
            isUIVisible = false;
        }

        StartCoroutine(BeginPlay());

        // ���ԣ���ӡ�ֱ�����״̬
        Debug.Log("���ֱ��Ƿ����ӣ�" + IsControllerConnected(XRNode.LeftHand));
        Debug.Log("���ֱ��Ƿ����ӣ�" + IsControllerConnected(XRNode.RightHand));
    }

    void Update()
    {
        // �ָ���������߼��������ӵ�����־
        bool leftButtonState = CheckButtonPressed(controllerNodeL, triggerButton);
        bool rightButtonState = CheckButtonPressed(controllerNodeR, triggerButton);
        bool currentButtonState = leftButtonState || rightButtonState;

        // ���ԣ���ӡ����״̬
        if (currentButtonState)
        {
            Debug.Log("��⵽��Ӧ�������£�");
        }

        if (currentButtonState && (Time.unscaledTime - lastTriggerTime > debounceTime))
        {
            ToggleUI();
            Debug.Log("�ɹ�������ͣ����ǰpaused״̬��" + paused);
            lastTriggerTime = Time.unscaledTime;
        }
    }

    private IEnumerator BeginPlay()
    {
        for (int i = 0; i < 4; i++)
        {
            btext.text = i != 3 ? (3 - i).ToString() : "Go!!!";
            yield return new WaitForSeconds(1.0f);
        }

        MaskUI.SetActive(false);
        emitter.SetActive(true);
    }

    public void backGame()
    {
        ToggleUI();
    }

    public void backMenu()
    {

        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    public void rePlay()
    {
        Time.timeScale = 1.0f;
        paused = false;

        LoginManager.Instance.WriteCSVLine(new string[] { "" });

        if (ChartLoader.levelName != "songnew")
        {
            LoginManager.Instance.WriteCSVLine(new string[] { $"进入关卡{ChartLoader.levelName}", DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") });
        } 
 LoginManager.Instance.WriteCSVLine(new string[] { "名称","数据"});
        SceneManager.LoadScene(currentscenename);
    }

    // ��ǿ��������߼��������豸�пպ͵���
    private bool CheckButtonPressed(XRNode node, InputHelpers.Button button)
    {
        // ��ȡXR�豸���пձ��������
        if (!InputDevices.GetDeviceAtXRNode(node).isValid)
        {
            Debug.LogWarning("δ��⵽" + node + "�ֱ����������ӣ�");
            return false;
        }

        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        bool isPressed = false;
        // ��ⰴ�����£����ӷ���ֵ�жϣ�
        bool success = device.IsPressed(button, out isPressed);
        if (!success)
        {
            Debug.LogWarning("�޷����" + node + "�ֱ���" + button + "������");
        }
        return isPressed;
    }

    // ��������������ֱ��Ƿ�����
    private bool IsControllerConnected(XRNode node)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        return device.isValid;
    }

    // �淶��ͣ�߼���Time.timeScale��Ϊ0����ȫ��ͣ��
    private void ToggleUI()
    {
        if (uiCanvas == null) return;
        if (Gameover) return;

        isUIVisible = !isUIVisible;


        uiCanvas.SetActive(isUIVisible);
        if (isUIVisible)
        {
            paused = true;
            audioSource.Pause();
            //Time.timeScale = 0.1f;
            RightPause.SetActive(true);
            LeftPause.SetActive(true);
            RightControl.SetActive(false);
            LeftControl.SetActive(false);
        }
        else
        {
            paused = false;
            audioSource.Play();
            Time.timeScale = 1f; // �ָ������ٶ�
            RightPause.SetActive(false);
            LeftPause.SetActive(false);
            RightControl.SetActive(true);
            LeftControl.SetActive(true);
        }
    }
}