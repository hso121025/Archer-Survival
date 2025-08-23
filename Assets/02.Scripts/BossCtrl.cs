using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossCtrl : MonoBehaviour
{
    public GameObject[] Item;
    GameObject ItemSpawn;
    bool isDie = false;

    BossShootCtrl bossShoot;

    private Transform PlayerPos;
    private CharacterController controller;

    public Image BossHPImg;
    float BossinitHp;

    float MoveSpeed = 2.0f;

    private Animator animator;
    float damage;

    public float BossHp = 20000f;

    float ShootCool = 0.0f;
    float ResetShoot = 1.0f;    //기본공격속도
    public float ShootingTime = 0.5f;  //1초마다 발사
    float ShootingTimer = 10f;   //5초동안 지속

    int ShootCount = 0;

    float DashTimer = 3f;
    float ResetDash = 3f;

    float gravity = -20f;
    Vector3 velocity;

    public enum BossAtack { Default, Dash, Shooting, RoundShoot }
    public BossAtack bossAttack = BossAtack.Default;

    void Awake()
    {
        animator = GetComponent<Animator>();
        bossShoot = GetComponent<BossShootCtrl>();
        controller = GetComponent<CharacterController>();
        PlayerPos = GameObject.FindWithTag("Player").transform;
    }

    void OnEnable()
    {
        isDie = false;
    }

    void Start()
    {
        BossinitHp = BossHp;
    }

    void Update()
    {
        BossAction();
        MonActionUpdate();
        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y = 0;
        }   
    }

    void MonActionUpdate()
    {
        if (isDie) return;

        Vector3 move = Vector3.zero;

        if (bossAttack != BossAtack.Dash)
        {
            Vector3 dir = PlayerPos.position - transform.position;
            dir.y = 0;
            if (dir.magnitude > 0.1f)
            {
                move = dir.normalized * MoveSpeed;

                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 7f);
            }
        }
        else
        {
            move = transform.forward * MoveSpeed;
        }

        controller.Move(move * Time.deltaTime);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Bullet"))
        {
            damage = coll.GetComponent<Bullet_Ctrl>().damage;
            TakeDamage();
            BulletPool_Mgr.Inst.ReturnToPool(coll.gameObject);
        }
    }

    public void TakeDamage()
    {
        BossHp -= damage;
        BossHPImg.fillAmount = BossHp / BossinitHp;

        if (BossHp <= 0)
        {
            MonsterDie();
            BossHp = 0;
        }
    }

    public void MonsterDie()
    {
        ItemGenerator();
        if (GameMgr.Inst != null)
            GameMgr.Inst.Kill();

        SceneManager.LoadScene("05.ClearScene");
    }

    void ItemGenerator()
    {
        for (int i = 2; i < 5; i++)
        {
            ItemSpawn = Instantiate(Item[i]);
            float offsetX = Random.Range(-5f, 5f);
            float offsetZ = Random.Range(-5f, 5f);
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f + new Vector3(offsetX, 0, offsetZ);
            ItemSpawn.transform.position = spawnPos;
        }
    }

    IEnumerator RoundShootRoutine()
    {
        yield return StartCoroutine(bossShoot.Create360Bullets(3));
    }

    void BossAction()
    {
        switch (bossAttack)
        {
            case BossAtack.Default:
                ShootCool -= Time.deltaTime;
                if (ShootCool <= 0.0f)
                {
                    StartCoroutine(bossShoot.CreateBullet());
                    ShootCount++;
                    ShootCool = ResetShoot;
                }
                if (ShootCount == 3)
                    bossAttack = BossAtack.Dash;
                break;

            case BossAtack.Dash:
                DashTimer -= Time.deltaTime;
                if (DashTimer >= 0)
                {
                    MoveSpeed = 20f;
                    ShootCount = 0;
                }
                else
                {
                    MoveSpeed = 2f;
                    bossAttack = BossAtack.Shooting;
                    DashTimer = ResetDash;
                }
                break;

            case BossAtack.Shooting:
                ShootingTimer -= Time.deltaTime;
                if (ShootingTimer >= 0)
                {
                    ShootCool = ShootingTime;
                    ShootingTime -= Time.deltaTime;
                    if (ShootCool <= 0.0f)
                    {
                        StartCoroutine(bossShoot.CreateBullet());
                        ShootingTime = 0.3f;
                    }
                }
                else
                {
                    bossAttack = BossAtack.RoundShoot;
                    ShootingTimer = 5.0f;
                }
                break;

            case BossAtack.RoundShoot:
                StartCoroutine(RoundShootRoutine());
                bossAttack = BossAtack.Default;
                break;
        }
    }
}

