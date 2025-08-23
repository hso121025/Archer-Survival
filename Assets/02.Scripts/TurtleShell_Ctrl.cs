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

    //--- Player �ٴ� ������ ����
    public float raycastDistance = 10f;
    public LayerMask groundLayer;
    //--- Player �ٴ� ������ ����

    float damage;

    float MonsterHp;

    //���� ����
    private bool isCharging = false;
    private Vector3 chargeDirection;
    public float chargeSpeed = 25f;

    public LayerMask obstacleLayer; // ��/��ֹ� ���̾�

    void Awake()
    {

        //������ Transform �Ҵ�
        MonsterPos = this.gameObject.GetComponent<Transform>();
        //���� ����� Player�� Transform �Ҵ�
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
            if (distance < 80f)  // �÷��̾�� 8���� ���� �Ÿ��� �� ����
            {
                StartCharge();
            }
        }
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


    void TurtleShellActionUpdate()
    {
        if (isDie == true)
            return;


        //���� �̵� ����
        float MoveVelocity = 15.0f;    //��� �ʴ� �̵� �ӵ�...

        Vector3 MoveDir = PlayerPos.position - MonsterPos.position;
        MoveDir.y = 0.0f;

        if (0.0f < MoveDir.magnitude)
        {
            Vector3 StepVec = MoveDir.normalized * MoveVelocity * Time.deltaTime;
            transform.Translate(StepVec, Space.World);

            //-- �̵� ������ �ٶ� ������ ó��
            float RotSpeed = 7.0f;  //�ʴ� ȸ�� �ӵ�
            Quaternion TargetRot = Quaternion.LookRotation(MoveDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotSpeed);
            //-- �̵� ������ �ٶ� ������ ó��
        }
    }
    void OnTriggerEnter(Collider coll)
    {
        // �Ѿ� �ǰ� ó��
        if (coll.gameObject.CompareTag("Bullet"))
        {
            damage = coll.gameObject.GetComponent<Bullet_Ctrl>().damage;
            TakeDamage();
        }

        // ���� ���� ���� �÷��̾�� �浹 ó��
        else if (coll.gameObject.CompareTag("Player") && isCharging && !isDie)
        {
            PlayerCtrl player = coll.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(10f); // ���ϴ� ������ ��ġ
                Debug.Log("�÷��̾�� ���� ������ ����!");
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

        //����ġ 90, ���� 7, �ڼ� 1, ��Ʈ 1, ��ź 1 Ȯ���� ����
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
            yield return new WaitForSeconds(0.05f); // 0.1�� ��¦
            monsterRenderer.material = Default;
        }
    }

    public void StartCharge()
    {
        if (isDie) return;

        isCharging = true;
        chargeDirection = (PlayerPos.position - transform.position).normalized;
        chargeDirection.y = 0f;

        // ���� ����
        Quaternion targetRot = Quaternion.LookRotation(chargeDirection);
        transform.rotation = targetRot;
    }

    void ChargeForward()
    {
        // ������ �̵�
        transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime, Space.World);

        // ���� �浹 ���� (�浹 �� Ǯ�� ����)
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, chargeDirection, out RaycastHit hit, 0.7f, obstacleLayer))
        {
            isCharging = false;
            isDie = true;

            // �浹 �� ����Ʈ, ���� � ����
            MonsterDie();

            // Ǯ�� ����
            MonsterPool_Mgr.Inst.ReturnToPool(this.gameObject);
        }
    }
}
