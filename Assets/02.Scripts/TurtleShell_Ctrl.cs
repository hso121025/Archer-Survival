using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TurtleShell_Ctrl : MonoBehaviour
{
    public GameObject[] Item;
    GameObject ItemSpawn;
    int RanItem;

    private Renderer monsterRenderer;

    public Material Default;
    public Material Hit;

    bool isDie = false;

    private Transform MonsterPos;
    private Transform PlayerPos;

    //--- Player 바닥 고정용 변수
    public float raycastDistance = 10f;
    public LayerMask groundLayer;
    //--- Player 바닥 고정용 변수

    float damage;

    float MonsterHp;

    //돌진 변수
    private bool isCharging = false;
    private Vector3 chargeDirection;
    public float chargeSpeed = 25f;

    public LayerMask obstacleLayer; // 벽/장애물 레이어

    void Awake()
    {

        //몬스터의 Transform 할당
        MonsterPos = this.gameObject.GetComponent<Transform>();
        //추적 대상인 Player의 Transform 할당
        PlayerPos = GameObject.FindWithTag("Player").GetComponent<Transform>();

        monsterRenderer = GetComponentInChildren<Renderer>();
    }

    void OnEnable()
    {
        isDie = false;
        monsterRenderer.material = Default;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging)
        {
            ChargeForward();
        }
        else
        {
            TurtleShellActionUpdate();

            float distance = Vector3.Distance(transform.position, PlayerPos.position);
            if (distance < 80f)  // 플레이어와 8미터 이하 거리일 때 돌진
            {
                StartCharge();
            }
        }
    }

    void FixedUpdate()
    {
        //--- Player 바닥 고정
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        }
        //--- Player 바닥 고정
    }


    void TurtleShellActionUpdate()
    {
        if (isDie == true)
            return;


        //추적 이동 구현
        float MoveVelocity = 15.0f;    //평면 초당 이동 속도...

        Vector3 MoveDir = PlayerPos.position - MonsterPos.position;
        MoveDir.y = 0.0f;

        if (0.0f < MoveDir.magnitude)
        {
            Vector3 StepVec = MoveDir.normalized * MoveVelocity * Time.deltaTime;
            transform.Translate(StepVec, Space.World);

            //-- 이동 방향을 바라 보도록 처리
            float RotSpeed = 7.0f;  //초당 회전 속도
            Quaternion TargetRot = Quaternion.LookRotation(MoveDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotSpeed);
            //-- 이동 방향을 바라 보도록 처리
        }
    }
    void OnTriggerEnter(Collider coll)
    {
        // 총알 피격 처리
        if (coll.gameObject.CompareTag("Bullet"))
        {
            damage = coll.gameObject.GetComponent<Bullet_Ctrl>().damage;
            TakeDamage();
        }

        // 돌진 중일 때만 플레이어와 충돌 처리
        else if (coll.gameObject.CompareTag("Player") && isCharging && !isDie)
        {
            PlayerCtrl player = coll.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(10f); // 원하는 데미지 수치
                Debug.Log("플레이어에게 돌진 데미지 입힘!");
            }

            isCharging = false;
            isDie = true;
            MonsterDie();
            MonsterPool_Mgr.Inst.ReturnToPool(this.gameObject);
        }
    }

    public void TakeDamage()
    {
        MonsterHp -= damage;

        StartCoroutine(HitEffect());

        if (MonsterHp <= 0 && !isDie)
        {
            isDie = true;
            MonsterDie();
            MonsterHp = 0;
            MonsterPool_Mgr.Inst.ReturnToPool(this.gameObject);
        }
    }

    public void MonsterDie()
    {
        ItemGenerator();
        if (GameMgr.Inst != null)
            GameMgr.Inst.Kill();
    }

    void ItemGenerator()
    {
        RanItem = Random.Range(0, 100);

        //경험치 90, 코인 7, 자석 1, 하트 1, 폭탄 1 확률로 스폰
        if (RanItem <= 89)    //0 ~ 89 : EXP
            ItemSpawn = EXPPool_Mgr.Inst.GetEXP(this.transform.position + Vector3.up * 1.5f);
        else if (90 <= RanItem && RanItem <= 96)  //90 ~ 96 : StarCoin
            ItemSpawn = Instantiate(Item[1]);
        else if (97 == RanItem) //97 : Bomb
            ItemSpawn = Instantiate(Item[2]);
        else if (98 == RanItem) //98 : Magnet
            ItemSpawn = Instantiate(Item[3]);
        else                    //99 : Heart
            ItemSpawn = Instantiate(Item[4]);

        Vector3 SpawnPos = this.transform.position + Vector3.up * 1.5f;
        ItemSpawn.transform.position = SpawnPos;
    }
    public void Init()
    {
        MonsterHp = MonsGenerator.Inst.hp;
    }

    IEnumerator HitEffect()
    {
        if (monsterRenderer != null)
        {
            monsterRenderer.material = Hit;
            yield return new WaitForSeconds(0.05f); // 0.1초 반짝
            monsterRenderer.material = Default;
        }
    }

    public void StartCharge()
    {
        if (isDie) return;

        isCharging = true;
        chargeDirection = (PlayerPos.position - transform.position).normalized;
        chargeDirection.y = 0f;

        // 방향 고정
        Quaternion targetRot = Quaternion.LookRotation(chargeDirection);
        transform.rotation = targetRot;
    }

    void ChargeForward()
    {
        // 앞으로 이동
        transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime, Space.World);

        // 전방 충돌 감지 (충돌 시 풀로 복귀)
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, chargeDirection, out RaycastHit hit, 0.7f, obstacleLayer))
        {
            isCharging = false;
            isDie = true;

            // 충돌 시 이펙트, 사운드 등도 가능
            MonsterDie();

            // 풀로 복귀
            MonsterPool_Mgr.Inst.ReturnToPool(this.gameObject);
        }
    }
}
