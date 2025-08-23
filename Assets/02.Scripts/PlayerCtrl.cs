using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    Animator _animation;
    
    public float BulletDamage = 20f;  // �⺻��

    //--- Ű���� �̵� ���� ����
    float h = 0, v = 0;

    Vector3 MoveH;          //�¿� �̵� ���� ���� ����
    Vector3 MoveV;          //���Ʒ� �̵� ���� ���� ����
    Vector3 DirVec;           //�̵��Ϸ��� ���� ���� ����
    public float MoveSpeed = 10.0f;
    //--- Ű���� �̵� ���� ����

    //--- Player �̵��������� ȸ�� ���� ����
    public GameObject Player = null;
    Vector3 PlayerDir;
    public float RotSpeed = 15.0f;
    Quaternion TargetRot = Quaternion.identity; //ȸ�� ���� ����
    //--- Player �̵��������� ȸ�� ���� ����

    //--- Player�� Hp ���� ���� 
    //Player�� ���� ����
    public float Hp = 150f;
    //Player�� ���� �ʱ갪
    public float initHp;
    public float MaxHp;
    //Player�� Hp bar �̹���
    public Image imgHpbar;
    //--- Player�� Hp ���� ���� 

    //--- Player �ٴ� ������ ����
    public float raycastDistance = 2.5f;
    public LayerMask groundLayer;
    //--- Player �ٴ� ������ ����

    //--- JoyStick �̵����� ����
    float JoyMvLen = 0.0f;
    Vector3 JoyMvDir = Vector3.zero;
    //--- JoyStick �̵����� ����

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
        //���� �ʱ갪 ����
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
        //--- Player �ٴ� ����
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        }
        //--- Player �ٴ� ����
    }

    public void BasicMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //Player �̵�
        if (h != 0.0f || v != 0.0f) //�̵�
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
        //Player �̵� 

        //Player�� �̵��������� ȸ����Ű�� �ڵ�
        if (Player != null && 0.0001f < PlayerDir.magnitude)
        {
            TargetRot = Quaternion.LookRotation(PlayerDir);
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, TargetRot, Time.deltaTime * RotSpeed);
        }
        //Player�� �̵��������� ȸ����Ű�� �ڵ�
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

        //--- ���̽�ƽ �̵� �ڵ�
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

            //Image UI �׸��� fillAmount �Ӽ��� ������ ���� ������ �� ����
            imgHpbar.fillAmount = (float)Hp / (float)initHp;

            //�÷��̾��� hp�� 0���ϸ� ��� ó��
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

            //Image UI �׸��� fillAmount �Ӽ��� ������ ���� ������ �� ����
            imgHpbar.fillAmount = (float)Hp / (float)initHp;

            //�÷��̾��� hp�� 0���ϸ� ��� ó��
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

        Debug.Log("�÷��̾� �ǰ�! ���� HP: " + Hp);

        // ���� ������ ������Ʈ
        UpdateHpUI();

        // ���� ó��
        if (Hp <= 0)
        {
            GameMgr.Inst.PlayerDie();
        }
    }
}
