using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterPool_Mgr : MonoBehaviour
{
    public static MonsterPool_Mgr Inst;

    [SerializeField] private GameObject Monster1Prefab;
    [SerializeField] private GameObject Monster2Prefab;
    [SerializeField] private GameObject TurtlrShellPrefab;

    [SerializeField] private int InitPoolSize = 200;

    public Transform Monsters;

    private List<GameObject> MonsterPool = new List<GameObject>();
    private List<GameObject> TurtleShellPool = new List<GameObject>();

    public string currentSceneName = null;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        string currentSceneName = GlobalValue.Inst.SceneName;

        if (currentSceneName == "04.Boss")
            return;

        //UIScene을 먼저 로드해서 currentScene이 항상 UI씬이 되버림
        for (int i = 0; i < InitPoolSize; i++)
        {
            if (currentSceneName == "02.Stage1")
            {
                GameObject monster = Instantiate(Monster1Prefab, Monsters);
                monster.SetActive(false);
                MonsterPool.Add(monster);
            }
            else
            {
                GameObject monster = Instantiate(Monster2Prefab, Monsters);
                monster.SetActive(false);
                MonsterPool.Add(monster);
            }
        }
    }

    public GameObject GetMonster(Vector3 position, Quaternion rotation)
    {
        string currentSceneName = GlobalValue.Inst.SceneName;
        if (currentSceneName == "04.Boss")
            return null;

        bool spawnTurtleShell = false;

        if (currentSceneName == "03.Stage2")  // Stage2에서만 30% 확률
        {
            int rand = Random.Range(0, 100);  // 0~99
            if (rand < 10)
                spawnTurtleShell = true;
        }

        if (spawnTurtleShell)
        {
            // --- TurtleShell 풀에서 가져오기 ---
            foreach (var turtle in TurtleShellPool)
            {
                if (turtle == null) continue;
                if (!turtle.activeInHierarchy)
                {
                    turtle.transform.position = position;
                    turtle.transform.rotation = rotation;
                    turtle.SetActive(true);
                    return turtle;
                }
            }

            GameObject newTurtle = Instantiate(TurtlrShellPrefab, position, rotation, Monsters);
            newTurtle.SetActive(true);
            TurtleShellPool.Add(newTurtle);
            return newTurtle;
        }
        else
        {
            // --- 기존 몬스터 풀에서 가져오기 ---
            foreach (var monster in MonsterPool)
            {
                if (monster == null) continue;
                if (!monster.activeInHierarchy)
                {
                    monster.transform.position = position;
                    monster.transform.rotation = rotation;
                    monster.SetActive(true);
                    return monster;
                }
            }

            GameObject newMonster;
            if (currentSceneName == "02.Stage1")
                newMonster = Instantiate(Monster1Prefab, position, rotation, Monsters);
            else
                newMonster = Instantiate(Monster2Prefab, position, rotation, Monsters);

            newMonster.SetActive(true);
            MonsterPool.Add(newMonster);
            return newMonster;
        }

        //// 비활성화된 몬스터 찾기
        //foreach (var monster in MonsterPool)
        //{
        //    if (monster == null)
        //        continue; // 추가: Destroy된 monster 건너뜀
        //    if (!monster.activeInHierarchy)
        //    {
        //        monster.transform.position = position;
        //        monster.transform.rotation = rotation;
        //        monster.SetActive(true);
        //        return monster;
        //    }
        //}

        //// 못 찾으면 새로 생성해서 리스트에 추가
        //if (currentSceneName == "02.Stage1")
        //{
        //    GameObject newMonster = Instantiate(Monster1Prefab, Monsters);
        //    newMonster.transform.position = position;
        //    newMonster.transform.rotation = rotation;
        //    //newMonster.SetActive(true);
        //    MonsterPool.Add(newMonster);
        //    return newMonster;
        //}
        //else
        //{
        //    GameObject newMonster = Instantiate(Monster2Prefab, Monsters);
        //    newMonster.transform.position = position;
        //    newMonster.transform.rotation = rotation;
        //    //newMonster.SetActive(true);
        //    MonsterPool.Add(newMonster);
        //    return newMonster;
        //}
    }
    

    public void ReturnToPool(GameObject monster)
    {
        monster.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
