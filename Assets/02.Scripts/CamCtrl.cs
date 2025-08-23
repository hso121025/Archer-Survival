using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //--- Cam �̵� ���� ����
    public Transform Player; // �÷��̾��� Transform
    //public Vector3 offset = new Vector3(0, 5, -7); // ī�޶� ��ġ ������
    public float smoothSpeed = 5f; // ī�޶� �̵� �ӵ�
    //--- Cam �̵� ���� ����

    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (Player == null)
           return;

        // ��ǥ ��ġ ���
        Vector3 targetPos = Player.position;

        // �ε巴�� �̵�
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
