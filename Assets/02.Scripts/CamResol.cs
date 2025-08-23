using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamResol : MonoBehaviour
{
    public GameObject UI_MaskGroup = null;

    //����Ʈ�� ������ǥ VpW : Viewport To World
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

        if (scaleHeight < 1.0f) //���ΰ� �� ä������ �� ������ ���Ʒ��� ���� �� �ִ� ��Ȳ
        {
            //(���Ʒ��� ���͹ڽ��� ����)
            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else  //���ΰ� �� ä������ �� ������ �¿�� ���� �� �ִ� ��Ȳ
        {
            //(�¿�� ���͹ڽ��� ����)
            rect.width = scaleWidth;
            rect.x = (1.0f - scaleWidth) / 2.0f;
        }

        a_Cam.rect = rect;

        if (UI_MaskGroup != null)
            UI_MaskGroup.SetActive(true);

        //--- ��ũ���� ���� ��ǥ ���ϱ�
        m_VpZero = new Vector3(0.0f, 0.0f, 0.0f);
        m_VpWMin = a_Cam.ViewportToWorldPoint(m_VpZero);
        //ī�޶� ȭ�� �����ϴ�(ȭ�� �ּҰ�) �ڳ��� ���� ��ǥ

        m_VpOne = new Vector3(1.0f, 1.0f, 1.0f);
        m_VpWMax = a_Cam.ViewportToWorldPoint(m_VpOne);
        //ī�޶� ȭ�� �������(ȭ�� �ִ밪) �ڳ��� ���� ��ǥ
        //--- ��ũ���� ���� ��ǥ ���ϱ�

    }//void Start()

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void LateUpdate()
    {
        //--- Viewport�� ���� ��ǥ ���ϱ�
        m_VpWMin = Camera.main.ViewportToWorldPoint(m_VpZero);
        //ī�޶� ȭ�� �����ϴ�(ȭ�� �ּҰ�) �ڳ��� ������ǥ

        m_VpWMax = Camera.main.ViewportToWorldPoint(m_VpOne);
        //ī�޶� ȭ�� �������(ȭ�� �ִ밪) �ڳ��� ������ǥ
        //--- Viewport�� ���� ��ǥ ���ϱ�

    }//void LateUpdate()
}