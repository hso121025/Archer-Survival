using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //--- Cam 이동 관련 변수
    public Transform Player; // 플레이어의 Transform
    //public Vector3 offset = new Vector3(0, 5, -7); // 카메라 위치 오프셋
    public float smoothSpeed = 5f; // 카메라 이동 속도
    //--- Cam 이동 관련 변수

    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (Player == null)
           return;

        // 목표 위치 계산
        Vector3 targetPos = Player.position;

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
