using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Monster_Ctrl : MonoBehaviour
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
    public float raycastDistance = 2.5f;
    public LayerMask groundLayer;
    //--- Player 바닥 고정용 변수

    private Animator animator;

    float damage;

    float MonsterHp;

    public AudioClip MonsterDieSound;

    void Awake()
    {
        animator = this.gameObject.GetComponent<Animator>();

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
        MonActionUpdate();
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


    void MonActionUpdate()
    {
        if (isDie == true)
            return;


        //추적 이동 구현
        float MoveVelocity = 2.0f;    //평면 초당 이동 속도...

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
        if (coll.gameObject.tag == "Bullet")
        {
            //맞은 총알의 Damage를 추출해 몬스터 hp 차감
            damage = coll.gameObject.GetComponent<Bullet_Ctrl>().damage;
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        MonsterHp -= damage;    //데미지 만큼 hp - 하기

        StartCoroutine(HitEffect());  // << 추가!

        if (MonsterHp <= 0)
        {
            StartCoroutine(Sound_Mgr.Inst.MonsterSound());



            MonsterDie();
            MonsterHp = 0;
            MonsterPool_Mgr.Inst.ReturnToPool(this.gameObject);
            return;
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
}
