using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

/// <summary>
/// ͨ��UI����չ�������ű�
/// ֧�ִ�Scale(0,0,0)ƽ�����ɵ��Զ����С����ָ������ʱ��
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIExpandAnimation : MonoBehaviour
{
    public string gameName;
    [Header("��������")]
    [Tooltip("����Ŀ�����Ŵ�С������չ���ĳߴ磩")]
    public Vector3 targetScale = Vector3.one; // Ĭ��1����С
    [Tooltip("������ʱ�����룩")]
    public float animationDuration = 0.3f; // Ĭ��0.3�����
    [Tooltip("�Ƿ����û���Ч�����ö�������Ȼ��")]
    public bool useSmoothEase = true;

    public UnityEvent OnExpandFinish;

    private RectTransform _rectTransform;
    private Coroutine _expandCoroutine; // ��¼��ǰ����Э�̣���ֹ�ظ�ִ��

    private void Awake()
    {
        // ��ȡUI��RectTransform�����UIԪ�صĺ��������
        _rectTransform = GetComponent<RectTransform>();
        // ��ʼ����Ĭ�Ͻ�UI���أ�����Ϊ0��
        _rectTransform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        if (_rectTransform != null)
            PlayExpandAnimation();
        if (LoginManager.Instance != null && gameName!="")
        {
  LoginManager.Instance.CreateUserMessage();

        LoginManager.Instance.WriteCSVLine(new string[] { $"已选择游戏:{gameName}", DateTime.Now.ToString("yyyy-MM-dd_hh:mm:ss") });
        }
      

    }

    /// <summary>
    /// �ⲿ���ã�����չ������
    /// </summary>
    public void PlayExpandAnimation()
    {
        // ������ж�����ִ�У���ֹͣ���������
        if (_expandCoroutine != null)
        {
            StopCoroutine(_expandCoroutine);
        }
        // ���ó�ʼ״̬������Ϊ0��
        _rectTransform.localScale = Vector3.zero;
        // ��������Э��
        _expandCoroutine = StartCoroutine(ExpandCoroutine());
    }

    /// <summary>
    /// չ����������Э��
    /// </summary>
    private IEnumerator ExpandCoroutine()
    {
        float elapsedTime = 0f; // �����ŵ�ʱ��
        Vector3 startScale = Vector3.zero; // ��ʼ���ţ�0��

        while (elapsedTime < animationDuration)
        {
            // ������ȣ�0~1��
            float t = elapsedTime / animationDuration;

            // ������û�������Mathf.SmoothStepʵ��ƽ���Ļ��뻺�����������Բ�ֵ
            float progress = useSmoothEase ? Mathf.SmoothStep(0f, 1f, t) : t;

            // ��ֵ���㵱ǰ����ֵ
            _rectTransform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            // �ۼ�ʱ�䣨Time.deltaTime��֤֡���޹أ�
            elapsedTime += Time.deltaTime;

            //�ȴ���һ֡
            yield return null;
        }

        // ����������ǿ������ΪĿ��ֵ�����⾫�����
        _rectTransform.localScale = targetScale;

        // ����Э�̱��
        _expandCoroutine = null;

        // ������ɻص�������չ��
        OnExpandAnimationComplete();
    }

    /// <summary>
    /// չ��������ɻص�������д��󶨣�
    /// </summary>
    protected virtual void OnExpandAnimationComplete()
    {
        Debug.Log($"{gameObject.name} չ��������ɣ�");
        OnExpandFinish?.Invoke();
    }
}