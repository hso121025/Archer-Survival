using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsGenerator : MonoBehaviour
{
    [HideInInspector] public Monster_Ctrl mon = null;
    [HideInInspector] public TurtleShell_Ctrl TurtleShell = null;

    public GameObject MonPrefab;
    public GameObject MonPrefab2;
    public GameObject MonPrefab3;

    float MonSpawn = 0.0f;
    float MonSpawnTime = 0.05f;

    public int maxMon = 500;

    public float hp = 20;

    Vector3 SpawnPos;

    int KILL;

    //--- �̱��� ������ ���� �ν��Ͻ� ���� ����
    public static MonsGenerator Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- �̱��� ������ ���� �ν��Ͻ� ���� ����

    void Start()
    {
        KILL = 0;

        mon = GameObject.FindAnyObjectByType<Monster_Ctrl>();
        TurtleShell = GameObject.FindAnyObjectByType<TurtleShell_Ctrl>();

        hp = GlobalValue.Inst.monsterHp;
        MonSpawnTime = GlobalValue.Inst.monsterSpawnTime;
        maxMon = GlobalValue.Inst.monsterMaxCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < MonSpawn)            //fireTimer�� 0.0���� ũ�� �ð����� -�Ѵ�
            MonSpawn -= Time.deltaTime;

        if (MonSpawn <= 0.0f)      //fireTimer�� 0.0���� �۰ų� ������ Fire�Լ��� ȣ���� ��, fireTimer�� firecool�� �ʱ�ȭ(0.11�� ��Ÿ���� �ش�)
        {
            {
                CreateMonster();
                MonSpawn = MonSpawnTime;
            }
        }

        MonsterLevelUp();
    }

    public void CreateMonster()
    {
        Vector3 SpawnPos = new Vector3(Random.Range(-127, 127), 5, Random.Range(-127, 127));

        GameObject mon = MonsterPool_Mgr.Inst.GetMonster(SpawnPos, Quaternion.identity);

        if (mon == null)
        {
            // 04.Boss ���� ���� ���� ������ ���ܵǹǷ� ���⼭ �����ϰų� ó�� �ǳʶ�
            return;
        }

        Monster_Ctrl[] monsters = FindObjectsOfType<Monster_Ctrl>();

        if (monsters.Length >= maxMon)
            return;

        // � Ÿ������ Ȯ�� �� Init
        Monster_Ctrl normal = mon.GetComponent<Monster_Ctrl>();
        if (normal != null)
        {
            normal.Init();
        }
        else
        {
            TurtleShell_Ctrl turtle = mon.GetComponent<TurtleShell_Ctrl>();
            if (turtle != null)
                turtle.Init();
        }
    }

    void MonsterLevelUp()
    {
        if(KILL == GameMgr.Inst.KillCount / 10)
            return;

        hp += 5f;

        if (MonSpawnTime >= 0.1f)
        {
            MonSpawnTime *= 0.9f;

            if (MonSpawnTime < 0.1f)
                MonSpawnTime = 0.1f;
        }

        maxMon += 10;

        GlobalValue.Inst.monsterHp = hp;
        GlobalValue.Inst.monsterSpawnTime = MonSpawnTime;
        GlobalValue.Inst.monsterMaxCount = maxMon;
        GlobalValue.Inst.SaveMonsterStats();

        KILL = GameMgr.Inst.KillCount / 10;
    }
}
