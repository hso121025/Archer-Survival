using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamResol : MonoBehaviour
{
    public GameObject UI_MaskGroup = null;

    //뷰포트의 월드좌표 VpW : Viewport To World
    public static Vector3 m_VpWMin = new Vector3(-10.0f, -5.0f, 0.0f);
    public static Vector3 m_VpWMax = new Vector3(10.0f, 5.0f, 0.0f);

    Vector3 m_VpZero = Vector3.zero;
    Vector3 m_VpOne = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        Camera a_Cam = GetComponent<Camera>();
        Rect rect = a_Cam.rect;
        float scaleHeight = ((float)Screen.width / Screen.height) /
                             ((float)16 / 9);
        float scaleWidth = 1.0f / scaleHeight;

        if (scaleHeight < 1.0f) //가로가 꽉 채워지고 빈 공간이 위아래로 생길 수 있는 상황
        {
            //(위아래로 레터박스가 생김)
            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else  //세로가 꽉 채워지고 빈 공간이 좌우로 생길 수 있는 상황
        {
            //(좌우로 레터박스가 생김)
            rect.width = scaleWidth;
            rect.x = (1.0f - scaleWidth) / 2.0f;
        }

        a_Cam.rect = rect;

        if (UI_MaskGroup != null)
            UI_MaskGroup.SetActive(true);

        //--- 스크린의 월드 좌표 구하기
        m_VpZero = new Vector3(0.0f, 0.0f, 0.0f);
        m_VpWMin = a_Cam.ViewportToWorldPoint(m_VpZero);
        //카메라 화면 좌측하단(화면 최소값) 코너의 월드 좌표

        m_VpOne = new Vector3(1.0f, 1.0f, 1.0f);
        m_VpWMax = a_Cam.ViewportToWorldPoint(m_VpOne);
        //카메라 화면 우측상단(화면 최대값) 코너의 월드 좌표
        //--- 스크린의 월드 좌표 구하기

    }//void Start()

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void LateUpdate()
    {
        //--- Viewport의 월드 좌표 구하기
        m_VpWMin = Camera.main.ViewportToWorldPoint(m_VpZero);
        //카메라 화면 좌측하단(화면 최소값) 코너의 월드좌표

        m_VpWMax = Camera.main.ViewportToWorldPoint(m_VpOne);
        //카메라 화면 우측상단(화면 최대값) 코너의 월드좌표
        //--- Viewport의 월드 좌표 구하기

    }//void LateUpdate()
}