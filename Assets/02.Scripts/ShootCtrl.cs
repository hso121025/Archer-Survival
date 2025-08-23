using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCtrl : MonoBehaviour
{
    public GameObject bullet;   //총알 프리팹
    public Transform FirePos;   //총알 발사 좌표

    PlayerCtrl player;

    float FireTime = 0.0f;
    float Firecool = 0.5f;

    Transform nearestMonster = null;

    public LayerMask EnemyLayer; // 몬스터 레이어 설정

    AudioSource ShootSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameMgr.Inst.Player.GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < FireTime)            //fireTimer이 0.0보다 크면 시간값을 -한다
            FireTime -= Time.deltaTime;

        if (FireTime <= 0.0f && nearestMonster != null && FirePos != null)      //fireTimer가 0.0보다 작거나 같으면 Fire함수를 호출한 후, fireTimer를 firecool로 초기화(0.11초 쿨타임을 준다)
        {
            ShootSound = GetComponent<AudioSource>();
            ShootSound.Play();
            CreateBullet();
            FireTime = Firecool;
        }

        FindNearestMonster();
    }

    void FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy"); // 몬스터들은 "Enemy" 태그로 지정되어 있어야 함
        float shortestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject monster in monsters)
        {
            float dist = Vector3.Distance(transform.position, monster.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closest = monster.transform;
            }
        }

        nearestMonster = closest;
    }

    void CreateBullet()
    {
        //수평 방향으로만 회전 계산
        Vector3 targetPos = new Vector3(nearestMonster.position.x, FirePos.position.y, nearestMonster.position.z);
        Vector3 dir = (targetPos - FirePos.position).normalized;
        Quaternion Rot = Quaternion.LookRotation(dir);

        if (nearestMonster != null)
        {
            GameObject bullet = BulletPool_Mgr.Inst.GetBullet(FirePos.position, Rot);
            bullet.GetComponent<Bullet_Ctrl>().Init(dir, player.BulletDamage);

            if (player.Skill_F2)
            {
                int count = player.FireBall_F2_Level;
                float spacing = 3f;
                float startOffset = -(spacing * (count - 1)) / 2f;
                Vector3 rightDir = Vector3.Cross(Vector3.up, dir).normalized;

                for (int i = 0; i < count; i++)
                {
                    float offset = ((i / 2) + 1) * spacing * (i % 2 == 0 ? -1 : 1);
                    Vector3 spawnPos = FirePos.position + rightDir * offset;

                    GameObject b = BulletPool_Mgr.Inst.GetBullet(spawnPos, Rot);
                    b.GetComponent<Bullet_Ctrl>().Init(dir, player.BulletDamage);
                }
            }

            if (player.Skill_FB)
            {
                int count = player.FireBall_FB_Level;
                float spacing = 3f;
                float startOffset = -(spacing * (count - 1)) / 2f;
                Vector3 BackDir = Vector3.Cross(Vector3.up, -dir).normalized; // ← 반대 방향 기준으로 횡 방향 설정
                Quaternion backRot = Quaternion.LookRotation(-dir);

                for (int i = 0; i < count; i++)
                {
                    Vector3 offset = BackDir * (startOffset + spacing * i);
                    Vector3 spawnPos = FirePos.position - dir * 2f + offset; // 뒤쪽 + 퍼짐

                    GameObject b = BulletPool_Mgr.Inst.GetBullet(spawnPos, backRot);
                    b.GetComponent<Bullet_Ctrl>().Init(-dir, player.BulletDamage);
                }
            }

            if (player.Skill_FF)
            {
                if (player.Skill_FF)
                {
                    StartCoroutine(Fire_FF_Coroutine(dir));
                }
            }

            if (player.Skill_RL)
            {
                int count = player.FireBall_RL_Level;
                float spacing = 3f;
                float startOffset = -(spacing * (count - 1)) / 2f;

                // "왼쪽 방향" (dir을 기준으로 90도 회전)
                Vector3 leftDir_RL = Vector3.Cross(Vector3.up, dir).normalized;
                // "오른쪽 방향" (dir을 기준으로 -90도 회전)
                Vector3 rightDir_RL = Vector3.Cross(dir, Vector3.up).normalized;

                // 왼쪽 발사
                for (int i = 0; i < count; i++)
                {
                    float offset = startOffset + spacing * i;
                    Vector3 spawnPos = FirePos.position + leftDir_RL * 2f + (Vector3.Cross(Vector3.up, leftDir_RL) * offset); // 왼쪽으로 나간 다음 좌우 퍼뜨리기
                    Quaternion rot = Quaternion.LookRotation(leftDir_RL);

                    GameObject b = BulletPool_Mgr.Inst.GetBullet(spawnPos, rot);
                    b.GetComponent<Bullet_Ctrl>().Init(leftDir_RL, player.BulletDamage);
                }

                // 오른쪽 발사
                for (int i = 0; i < count; i++)
                {
                    float offset = startOffset + spacing * i;
                    Vector3 spawnPos = FirePos.position + rightDir_RL * 2f + (Vector3.Cross(Vector3.up, rightDir_RL) * offset); // 오른쪽으로 나간 다음 좌우 퍼뜨리기
                    Quaternion rot = Quaternion.LookRotation(rightDir_RL);

                    GameObject b = BulletPool_Mgr.Inst.GetBullet(spawnPos, rot);
                    b.GetComponent<Bullet_Ctrl>().Init(rightDir_RL, player.BulletDamage);
                }
            }


            if (player.Skill_RL2)
            {
                int count = player.FireBall_RL2_Level;

                float angleSpacing = 15f; // 총알 하나당 15도 간격
                Vector3 forward = dir.normalized; // 정면 방향 (기준 방향)

                for (int i = 0; i < count; i++)
                {
                    // 왼쪽 발사
                    float leftAngle = -angleSpacing * (i + 1);
                    Quaternion leftRot = Quaternion.AngleAxis(leftAngle, Vector3.up) * Quaternion.LookRotation(forward);
                    Vector3 leftDir_RL2 = leftRot * Vector3.forward;

                    GameObject bulletL = BulletPool_Mgr.Inst.GetBullet(FirePos.position, leftRot);
                    bulletL.GetComponent<Bullet_Ctrl>().Init(leftDir_RL2.normalized, player.BulletDamage);
                }

                for (int i = 0; i < count; i++)
                {
                    // 오른쪽 발사
                    float rightAngle = angleSpacing * (i + 1);
                    Quaternion rightRot = Quaternion.AngleAxis(rightAngle, Vector3.up) * Quaternion.LookRotation(forward);
                    Vector3 rightDir_RL2 = rightRot * Vector3.forward;

                    GameObject bulletR = BulletPool_Mgr.Inst.GetBullet(FirePos.position, rightRot);
                    bulletR.GetComponent<Bullet_Ctrl>().Init(rightDir_RL2.normalized, player.BulletDamage);
                }
            }
        }
    }

    IEnumerator Fire_FF_Coroutine(Vector3 dir)
    {
        Vector3 targetPos = new Vector3(nearestMonster.position.x, FirePos.position.y, nearestMonster.position.z);
        Vector3 Dir = (targetPos - FirePos.position).normalized;
        Quaternion Rot = Quaternion.LookRotation(Dir);

        int repeatCount = player.FireBall_FF_Level;

        for (int i = 0; i < repeatCount; i++)
        {
            yield return new WaitForSeconds(0.1f); // 0.2초 대기

            GameObject bullet = BulletPool_Mgr.Inst.GetBullet(FirePos.position, Rot);
            bullet.GetComponent<Bullet_Ctrl>().Init(dir, player.BulletDamage);

            // 추가 발사
            if (player.Skill_F2)
                Fire_F2(dir);

            if (player.Skill_RL2)
                Fire_RL2(dir);
        }
    }

    void Fire_F2(Vector3 dir)
    {
        int count = player.FireBall_F2_Level;
        float spacing = 3f;
        float startOffset = -(spacing * (count - 1)) / 2f;
        Vector3 rightDir = Vector3.Cross(Vector3.up, dir).normalized;

        for (int i = 0; i < count; i++)
        {
            float offset = ((i / 2) + 1) * spacing * (i % 2 == 0 ? -1 : 1);
            Vector3 spawnPos = FirePos.position + rightDir * offset;
            Quaternion rot = Quaternion.LookRotation(dir);

            GameObject b = BulletPool_Mgr.Inst.GetBullet(spawnPos, rot);
            b.GetComponent<Bullet_Ctrl>().Init(dir, player.BulletDamage);
        }
    }

    void Fire_RL2(Vector3 dir)
    {
        int count = player.FireBall_RL2_Level;
        if (count <= 0)
            return;

        float angleSpacing = 15f;
        Vector3 forward = dir.normalized;

        for (int i = 0; i < count; i++)
        {
            float leftAngle = -angleSpacing * (i + 1);
            Quaternion leftRot = Quaternion.AngleAxis(leftAngle, Vector3.up) * Quaternion.LookRotation(forward);
            Vector3 leftDir = leftRot * Vector3.forward;

            GameObject bulletL = BulletPool_Mgr.Inst.GetBullet(FirePos.position, leftRot);
            bulletL.GetComponent<Bullet_Ctrl>().Init(leftDir.normalized, player.BulletDamage);
        }

        for (int i = 0; i < count; i++)
        {
            float rightAngle = angleSpacing * (i + 1);
            Quaternion rightRot = Quaternion.AngleAxis(rightAngle, Vector3.up) * Quaternion.LookRotation(forward);
            Vector3 rightDir = rightRot * Vector3.forward;

            GameObject bulletR = BulletPool_Mgr.Inst.GetBullet(FirePos.position, rightRot);
            bulletR.GetComponent<Bullet_Ctrl>().Init(rightDir.normalized, player.BulletDamage);
        }
    }
}

