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

    //--- 싱글턴 패턴을 위한 인스턴스 변수 선언
    public static MonsGenerator Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- 싱글턴 패턴을 위한 인스턴스 변수 선언

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
        if (0.0f < MonSpawn)            //fireTimer이 0.0보다 크면 시간값을 -한다
            MonSpawn -= Time.deltaTime;

        if (MonSpawn <= 0.0f)      //fireTimer가 0.0보다 작거나 같으면 Fire함수를 호출한 후, fireTimer를 firecool로 초기화(0.11초 쿨타임을 준다)
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
            // 04.Boss 씬일 때는 몬스터 생성이 차단되므로 여기서 리턴하거나 처리 건너뜀
            return;
        }

        Monster_Ctrl[] monsters = FindObjectsOfType<Monster_Ctrl>();

        if (monsters.Length >= maxMon)
            return;

        // 어떤 타입인지 확인 후 Init
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
