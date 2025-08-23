using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    Animator _animation;
    
    public float BulletDamage = 20f;  // 기본값

    //--- 키보드 이동 관련 변수
    float h = 0, v = 0;

    Vector3 MoveH;          //좌우 이동 보폭 계산용 변수
    Vector3 MoveV;          //위아래 이동 보폭 계산용 변수
    Vector3 DirVec;           //이동하려는 방향 벡터 변수
    public float MoveSpeed = 10.0f;
    //--- 키보드 이동 관련 변수

    //--- Player 이동방향으로 회전 관련 변수
    public GameObject Player = null;
    Vector3 PlayerDir;
    public float RotSpeed = 15.0f;
    Quaternion TargetRot = Quaternion.identity; //회전 계산용 변수
    //--- Player 이동방향으로 회전 관련 변수

    //--- Player의 Hp 관련 변수 
    //Player의 생명 변수
    public float Hp = 150f;
    //Player의 생명 초깃값
    public float initHp;
    public float MaxHp;
    //Player의 Hp bar 이미지
    public Image imgHpbar;
    //--- Player의 Hp 관련 변수 

    //--- Player 바닥 고정용 변수
    public float raycastDistance = 2.5f;
    public LayerMask groundLayer;
    //--- Player 바닥 고정용 변수

    //--- JoyStick 이동관련 변수
    float JoyMvLen = 0.0f;
    Vector3 JoyMvDir = Vector3.zero;
    //--- JoyStick 이동관련 변수

    public bool Skill_F2 = false;
    public bool Skill_FB = false;
    public bool Skill_FF = false;
    public bool Skill_RL = false;
    public bool Skill_RL2 = false;

    public int FireBall_F2_Level = 0;
    public int FireBall_FB_Level = 0;
    public int FireBall_FF_Level = 0;
    public int FireBall_RL_Level = 0;
    public int FireBall_RL2_Level = 0;

    

    

    // Start is called before the first frame update
    void Start()
    {
        //생명 초깃값 설정
        initHp = Hp;

        imgHpbar = GameObject.Find("HpBar").GetComponent<Image>();

        _animation = GetComponent<Animator>();

        GlobalValue.Inst.SkillUP();
    }

    // Update is called once per frame
    void Update()
    {
        BasicMove();

        JoyStickMvUpdate();
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

    public void BasicMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //Player 이동
        if (h != 0.0f || v != 0.0f) //이동
        {
            MoveH = Vector3.right * h;
            MoveV = Vector3.forward * v;

            DirVec = MoveH + MoveV;
            if (1.0f < DirVec.magnitude)
                DirVec.Normalize();

            transform.Translate(DirVec * MoveSpeed * Time.deltaTime, Space.World);

            PlayerDir = DirVec;

            _animation.SetFloat("Horizontal", h);
            _animation.SetFloat("Vertical", v);
        }
        //Player 이동 

        //Player를 이동방향으로 회전시키는 코드
        if (Player != null && 0.0001f < PlayerDir.magnitude)
        {
            TargetRot = Quaternion.LookRotation(PlayerDir);
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, TargetRot, Time.deltaTime * RotSpeed);
        }
        //Player를 이동방향으로 회전시키는 코드
    }

    public void SetJoyStickMv(Vector2 JoyDir)
    {
        _animation.SetFloat("Horizontal", JoyDir.x);
        _animation.SetFloat("Vertical", JoyDir.y);


        JoyMvLen = JoyDir.magnitude;
        JoyMvDir = new Vector3(JoyDir.x, 0.0f, JoyDir.y);

    }

    public void JoyStickMvUpdate()
    {
        if (h != 0.0f || v != 0.0f)
            return;

        //--- 조이스틱 이동 코드
        if (0.0f < JoyMvLen)
        {
            float mvVelocity = MoveSpeed;

            DirVec = JoyMvDir;
            transform.Translate(JoyMvDir * mvVelocity * Time.deltaTime, Space.World);

            if (Player != null && 0.0001f < DirVec.magnitude)
            {
                TargetRot = Quaternion.LookRotation(DirVec);
                Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, TargetRot, Time.deltaTime * RotSpeed);
            }
        }
    }

    void OnCollisionStay(Collision coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            Hp -= 0.5f;

            //Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
            imgHpbar.fillAmount = (float)Hp / (float)initHp;

            //플레이어의 hp가 0이하면 사망 처리
            if (Hp <= 0)
            {
                GameMgr.Inst.PlayerDie();
            }
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag != "EnemyBullet")
            return;
        else
        {
            Destroy(coll.gameObject, 0.1f);

            Hp -= 10.0f;

            //Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
            imgHpbar.fillAmount = (float)Hp / (float)initHp;

            //플레이어의 hp가 0이하면 사망 처리
            if (Hp <= 0)
            {
                GameMgr.Inst.PlayerDie();
            }
        }
    }

    public void UpdateHpUI()
    {
        imgHpbar.fillAmount = (float)Hp / (float)initHp;  
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
        if (Hp < 0) Hp = 0;

        Debug.Log("플레이어 피격! 현재 HP: " + Hp);

        // 생명 게이지 업데이트
        UpdateHpUI();

        // 죽음 처리
        if (Hp <= 0)
        {
            GameMgr.Inst.PlayerDie();
        }
    }
}
