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

    //--- 싱글턴 패턴
    public static FlexibleJoy_Mgr Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- 싱글턴 패턴


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
            Destroy(this); //지금 이 스크립트 제거
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
        //Js_Background	조이스틱 배경 (RectTransform) -- 이 안에서 좌표를 재계산하고 싶음
        //eventData.position : 마우스 좌표
        //pressEventCamera : 터치를 감지할 때 쓰는 카메라 (UI는 보통 Canvas에 연결된 카메라)
        //touchPos : 변환된 결과. 조이스틱 배경 기준의 위치 (중심을 (0,0)으로 하는 로컬 좌표계로 변환된 값)
        //조이스틱 배경(RectTransform) 중심을 기준으로 마우스 좌표를 얼마나 이동 시켰는지 알고 싶을 때
        //사용되는 함수
        Vector2 clampedPos = Vector2.ClampMagnitude(touchPos, Joy_Radius);
        JoyHandle.anchoredPosition = clampedPos;
        InputDirection = clampedPos / Joy_Radius;  //벡터의 최대 크기는 1.0f가 될 것임

        //캐릭터 이동 처리
        if (playerCtrl != null)
            playerCtrl.SetJoyStickMv(InputDirection);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        JoyBack.transform.position = OriginPos;
        JoyHandle.anchoredPosition = Vector2.zero; // 핸들 초기화 (원래 위치로...)

        if (JoyBack != null)
            JoyBack.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);
        if (JoyHandle != null)
            JoyHandle.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);

        //핸들 이미지를 로컬의 중심점으로 이동 시키겠다는 뜻
        if (JoyStick_Mgr.Inst.JoyStickType == JoyStickType.FlexibleOnOff)
        {
            JoyBack.gameObject.SetActive(false);
        }

        //캐릭터 이동 처리
        if (playerCtrl != null)
            playerCtrl.SetJoyStickMv(InputDirection);
    }

    public void ResetJoy()
    {
        InputDirection = Vector2.zero;
        JoyBack.transform.position = OriginPos;
        JoyHandle.anchoredPosition = Vector2.zero; // 핸들 초기화 (원래 위치로...)

        if (JoyBack != null)
            JoyBack.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);
        if (JoyHandle != null)
            JoyHandle.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 120);

        //핸들 이미지를 로컬의 중심점으로 이동 시키겠다는 뜻
        if (JoyStick_Mgr.Inst.JoyStickType == JoyStickType.FlexibleOnOff)
        {
            JoyBack.gameObject.SetActive(false);
        }
        playerCtrl.SetJoyStickMv(InputDirection);
    }
}
