using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCtrl : MonoBehaviour
{
    public GameObject bullet;   //�Ѿ� ������
    public Transform FirePos;   //�Ѿ� �߻� ��ǥ

    PlayerCtrl player;

    float FireTime = 0.0f;
    float Firecool = 0.5f;

    Transform nearestMonster = null;

    public LayerMask EnemyLayer; // ���� ���̾� ����

    AudioSource ShootSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameMgr.Inst.Player.GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < FireTime)            //fireTimer�� 0.0���� ũ�� �ð����� -�Ѵ�
            FireTime -= Time.deltaTime;

        if (FireTime <= 0.0f && nearestMonster != null && FirePos != null)      //fireTimer�� 0.0���� �۰ų� ������ Fire�Լ��� ȣ���� ��, fireTimer�� firecool�� �ʱ�ȭ(0.11�� ��Ÿ���� �ش�)
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
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy"); // ���͵��� "Enemy" �±׷� �����Ǿ� �־�� ��
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
        //���� �������θ� ȸ�� ���
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
                Vector3 BackDir = Vector3.Cross(Vector3.up, -dir).normalized; // �� �ݴ� ���� �������� Ⱦ ���� ����
                Quaternion backRot = Quaternion.LookRotation(-dir);

                for (int i = 0; i < count; i++)
                {
                    Vector3 offset = BackDir * (startOffset + spacing * i);
                    Vector3 spawnPos = FirePos.position - dir * 2f + offset; // ���� + ����

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

                // "���� ����" (dir�� �������� 90�� ȸ��)
                Vector3 leftDir_RL = Vector3.Cross(Vector3.up, dir).normalized;
                // "������ ����" (dir�� �������� -90�� ȸ��)
                Vector3 rightDir_RL = Vector3.Cross(dir, Vector3.up).normalized;

                // ���� �߻�
                for (int i = 0; i < count; i++)
                {
                    float offset = startOffset + spacing * i;
                    Vector3 spawnPos = FirePos.position + leftDir_RL * 2f + (Vector3.Cross(Vector3.up, leftDir_RL) * offset); // �������� ���� ���� �¿� �۶߸���
                    Quaternion rot = Quaternion.LookRotation(leftDir_RL);

                    GameObject b = BulletPool_Mgr.Inst.GetBullet(spawnPos, rot);
                    b.GetComponent<Bullet_Ctrl>().Init(leftDir_RL, player.BulletDamage);
                }

                // ������ �߻�
                for (int i = 0; i < count; i++)
                {
                    float offset = startOffset + spacing * i;
                    Vector3 spawnPos = FirePos.position + rightDir_RL * 2f + (Vector3.Cross(Vector3.up, rightDir_RL) * offset); // ���������� ���� ���� �¿� �۶߸���
                    Quaternion rot = Quaternion.LookRotation(rightDir_RL);

                    GameObject b = BulletPool_Mgr.Inst.GetBullet(spawnPos, rot);
                    b.GetComponent<Bullet_Ctrl>().Init(rightDir_RL, player.BulletDamage);
                }
            }


            if (player.Skill_RL2)
            {
                int count = player.FireBall_RL2_Level;

                float angleSpacing = 15f; // �Ѿ� �ϳ��� 15�� ����
                Vector3 forward = dir.normalized; // ���� ���� (���� ����)

                for (int i = 0; i < count; i++)
                {
                    // ���� �߻�
                    float leftAngle = -angleSpacing * (i + 1);
                    Quaternion leftRot = Quaternion.AngleAxis(leftAngle, Vector3.up) * Quaternion.LookRotation(forward);
                    Vector3 leftDir_RL2 = leftRot * Vector3.forward;

                    GameObject bulletL = BulletPool_Mgr.Inst.GetBullet(FirePos.position, leftRot);
                    bulletL.GetComponent<Bullet_Ctrl>().Init(leftDir_RL2.normalized, player.BulletDamage);
                }

                for (int i = 0; i < count; i++)
                {
                    // ������ �߻�
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
            yield return new WaitForSeconds(0.1f); // 0.2�� ���

            GameObject bullet = BulletPool_Mgr.Inst.GetBullet(FirePos.position, Rot);
            bullet.GetComponent<Bullet_Ctrl>().Init(dir, player.BulletDamage);

            // �߰� �߻�
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

