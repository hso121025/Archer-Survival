using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlexibleJoy_Mgr : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform JoyBack;
    public RectTransform JoyHandle;

    private Vector2 InputDirection;
    private float Joy_Radius;
    Vector3 OriginPos = Vector3.zero;

    [HideInInspector] public PlayerCtrl playerCtrl = null;

    //--- �̱��� ����
    public static FlexibleJoy_Mgr Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- �̱��� ����


    // Start is called before the first frame update
    void Start()
    {
        playerCtrl = FindObjectOfType<PlayerCtrl>();

        OriginPos = JoyBack.transform.position;
        Joy_Radius = JoyBack.sizeDelta.x * 0.34f;
        if (JoyStick_Mgr.Inst.JoyStickType == JoyStickType.FlexibleOnOff)
        {
            JoyBack.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCtrl != null && playerCtrl.Hp <= 0.0f)
        {
            Destroy(this); //���� �� ��ũ��Ʈ ����
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        JoyBack.gameObject.SetActive(true);
        JoyBack.position = eventData.position;
        JoyHandle.anchoredPosition = Vector2.zero;

        if (JoyBack != null)
            JoyBack.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        if (JoyHandle != null)
            JoyHandle.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            JoyBack, eventData.position, eventData.pressEventCamera, out touchPos);
        //Js_Background	���̽�ƽ ��� (RectTransform) -- �� �ȿ��� ��ǥ�� �����ϰ� ����
        //eventData.position : ���콺 ��ǥ
        //pressEventCamera : ��ġ�� ������ �� ���� ī�޶� (UI�� ���� Canvas�� ����� ī�޶�)
        //touchPos : ��ȯ�� ���. ���̽�ƽ ��� ������ ��ġ (�߽��� (0,0)���� �ϴ� ���� ��ǥ��� ��ȯ�� ��)
        //���̽�ƽ ���(RectTransform) �߽��� �������� ���콺 ��ǥ�� �󸶳� �̵� ���״��� �˰� ���� ��
        //���Ǵ� �Լ�
        Vector2 clampedPos = Vector2.ClampMagnitude(touchPos, Joy_Radius);
        JoyHandle.anchoredPosition = clampedPos;
        InputDirection = clampedPos / Joy_Radius;  //������ �ִ� ũ��� 1.0f�� �� ����

        //ĳ���� �̵� ó��
        if (playerCtrl != null)
            playerCtrl.SetJoyStickMv(InputDirection);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        JoyBack.transform.position = OriginPos;
        JoyHandle.anchoredPosition = Vector2.zero; // �ڵ� �ʱ�ȭ (���� ��ġ��...)

        if (JoyBack != null)
            JoyBack.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);
        if (JoyHandle != null)
            JoyHandle.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);

        //�ڵ� �̹����� ������ �߽������� �̵� ��Ű�ڴٴ� ��
        if (JoyStick_Mgr.Inst.JoyStickType == JoyStickType.FlexibleOnOff)
        {
            JoyBack.gameObject.SetActive(false);
        }

        //ĳ���� �̵� ó��
        if (playerCtrl != null)
            playerCtrl.SetJoyStickMv(InputDirection);
    }

    public void ResetJoy()
    {
        InputDirection = Vector2.zero;
        JoyBack.transform.position = OriginPos;
        JoyHandle.anchoredPosition = Vector2.zero; // �ڵ� �ʱ�ȭ (���� ��ġ��...)

        if (JoyBack != null)
            JoyBack.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);
        if (JoyHandle != null)
            JoyHandle.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);

        //�ڵ� �̹����� ������ �߽������� �̵� ��Ű�ڴٴ� ��
        if (JoyStick_Mgr.Inst.JoyStickType == JoyStickType.FlexibleOnOff)
        {
            JoyBack.gameObject.SetActive(false);
        }
        playerCtrl.SetJoyStickMv(InputDirection);
    }
}
