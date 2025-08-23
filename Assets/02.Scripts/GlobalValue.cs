using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalValue : MonoBehaviour
{
    public static GlobalValue Inst;

    public List<int> currentSkills = new List<int>();

    private const string SkillKey = "PlayerSkillList";
    private const string FirstItemGivenKey = "FirstItemGiven";

    public int totalCoin = 0;
    private const string CoinKey = "TotalCoin";

    public List<int> Items = new List<int>();
    private const string ItemKey = "ItemList";

    public int stage1kill;
    public int stage2kill;
    public int totalKill => stage1kill + stage2kill;

    private const string BestTotalKillKey = "BestTotalKill";

    public int playerLevel = 1;
    public int playerCurExp = 0;
    public float playerMaxExp = 100;

    private const string LevelKey = "PlayerLevel";
    private const string CurExpKey = "PlayerCurExp";
    private const string MaxExpKey = "PlayerMaxExp";

    public float monsterHp = 20f;
    public float monsterSpawnTime = 0.05f;
    public int monsterMaxCount = 500;

    private const string MonsterHpKey = "MonsterHP";
    private const string MonsterSpawnKey = "MonsterSpawnTime";
    private const string MonsterMaxKey = "MonsterMaxMon";

    public AudioClip Main_BGM;
    public AudioClip Stage01_BGM;
    public AudioClip Stage02_BGM;
    public AudioClip BOSS_BGM;

    public string SceneName;

    public Material BossSkybox;
    public Material Stage1Skybox;
    public Material Stage2Skybox;

    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
            LoadSkills();
            LoadCoin();
            LoadItems();
            LoadExpData();
            LoadMonsterStats();
            
        }
        else
        {
            Destroy(gameObject);
        }
        ResetKillCounts();
        ResetExpData();


    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneName == scene.name)
            return;

        SceneName = scene.name;

        if (SceneName == "02.Stage1")
        {
            if (GetComponent<AudioSource>().clip != Stage01_BGM)
                GetComponent<AudioSource>().clip = Stage01_BGM;

            if (Stage1Skybox != null)
            {
                RenderSettings.skybox = Stage1Skybox;
                DynamicGI.UpdateEnvironment();
            }
        }
        else if (SceneName == "03.Stage2")
        {
            if (GetComponent<AudioSource>().clip != Stage02_BGM)
                GetComponent<AudioSource>().clip = Stage02_BGM;

            if (Stage2Skybox != null)
            {
                RenderSettings.skybox = Stage2Skybox;
                DynamicGI.UpdateEnvironment();
            }
        }
        else if (SceneName == "04.Boss")
        {
            if (GetComponent<AudioSource>().clip != BOSS_BGM)
                GetComponent<AudioSource>().clip = BOSS_BGM;


            if (BossSkybox != null)
            {
                RenderSettings.skybox = BossSkybox;
                DynamicGI.UpdateEnvironment();
            }

        }
        else
        {
            if (GetComponent<AudioSource>().clip != Main_BGM)
                GetComponent<AudioSource>().clip = Main_BGM;
        }

        GetComponent<AudioSource>().Play();
    }

    public void AddItem(int itemID)
    {
        Items.Add(itemID);
        SaveItems();
    }

    public void SaveItems()
    {
        string saveData = string.Join(",", Items);
        PlayerPrefs.SetString(ItemKey, saveData);
        PlayerPrefs.Save();
    }

    public void LoadItems()
    {
        Items.Clear();
        string savedData = PlayerPrefs.GetString(ItemKey, "");

        string[] data = savedData.Split(',');
        foreach (string s in data)
        {
            if (int.TryParse(s, out int itemID))
                Items.Add(itemID);
        }

    }

    public void AddSkill(int skillID)
    {
        currentSkills.Add(skillID);
        SaveSkills();
    }

    public void ClearSkills()
    {
        currentSkills.Clear();
        PlayerPrefs.DeleteKey(SkillKey);
    }

    private void SaveSkills()
    {
        string saveData = string.Join(",", currentSkills);
        PlayerPrefs.SetString(SkillKey, saveData);
        PlayerPrefs.Save();
    }

    public void LoadSkills()
    {
        currentSkills.Clear();

        string savedData = PlayerPrefs.GetString(SkillKey, "");

        string[] data = savedData.Split(',');
        foreach (string s in data)
        {
            if (int.TryParse(s, out int skillID))
                currentSkills.Add(skillID);
        }
    }

    public void SkillUP()
    {
        List<int> copyList = new List<int>(currentSkills); // 복사본 사용
        foreach (int skill in copyList)
        {
            Skill_Mgr.Inst.SkillUpdate(skill); // 내부에서 AddSkill() 호출해도 안전
        }
    }

    public void ResetKillCounts()
    {
        stage1kill = 0;
        stage2kill = 0;
    }

    public void AddStage1Kill(int value = 1)
    {
        stage1kill += value;
    }
    public void AddStage2Kill(int value = 1)
    {
        stage2kill += value;
    }
    public int GetBestTotalKill()
    {
        return PlayerPrefs.GetInt(BestTotalKillKey, 0);
    }
    public void SaveBestTotalKillNew()
    {
        int best = GetBestTotalKill();
        if (totalKill > best)
        {
            PlayerPrefs.SetInt(BestTotalKillKey, totalKill);
            PlayerPrefs.Save();
        }
    }
    public void SaveExpData()
    {
        PlayerPrefs.SetInt(LevelKey, playerLevel);
        PlayerPrefs.SetInt(CurExpKey, playerCurExp);
        PlayerPrefs.SetFloat(MaxExpKey, playerMaxExp);
        PlayerPrefs.Save();
    }
    public void LoadExpData()
    {
        playerLevel = PlayerPrefs.GetInt(LevelKey, 1);
        playerCurExp = PlayerPrefs.GetInt(CurExpKey, 0);
        playerMaxExp = PlayerPrefs.GetFloat(MaxExpKey, 100f);
    }

    public void ResetExpData()
    {
        playerLevel = 1;
        playerCurExp = 0;
        playerMaxExp = 100f;

        PlayerPrefs.DeleteKey("PlayerLevel");
        PlayerPrefs.DeleteKey("PlayerCurExp");
        PlayerPrefs.DeleteKey("PlayerMaxExp");
    }

    public void SaveCoin()
    {
        PlayerPrefs.SetInt(CoinKey, totalCoin);
        PlayerPrefs.Save();
    }

    public void LoadCoin()
    {
        totalCoin = PlayerPrefs.GetInt(CoinKey, 0);
    }

    public void AddCoin(int getcoin)
    {
        totalCoin += getcoin += 9;
        SaveCoin(); // 저장까지 자동으로 처리
    }
    public void SaveMonsterStats()
    {
        PlayerPrefs.SetFloat(MonsterHpKey, monsterHp);
        PlayerPrefs.SetFloat(MonsterSpawnKey, monsterSpawnTime);
        PlayerPrefs.SetInt(MonsterMaxKey, monsterMaxCount);
        PlayerPrefs.Save();
    }
    public void LoadMonsterStats()
    {
        monsterHp = PlayerPrefs.GetFloat(MonsterHpKey, 20f);
        monsterSpawnTime = PlayerPrefs.GetFloat(MonsterSpawnKey, 0.05f);
        monsterMaxCount = PlayerPrefs.GetInt(MonsterMaxKey, 500);
    }
    public void ResetMonsterStats()
    {
        monsterHp = 20f;
        monsterSpawnTime = 0.05f;
        monsterMaxCount = 500;

        PlayerPrefs.DeleteKey(MonsterHpKey);
        PlayerPrefs.DeleteKey(MonsterSpawnKey);
        PlayerPrefs.DeleteKey(MonsterMaxKey);
    }

    public void GiveDefaultItemsOnce()
    {
        if (!PlayerPrefs.HasKey(FirstItemGivenKey))
        {
            // Heart: 1
            AddItem(1);
            // Bomb: 2
            AddItem(2);
            // Magnet: 3
            AddItem(3);

            PlayerPrefs.SetInt(FirstItemGivenKey, 1);
            PlayerPrefs.Save();

            Debug.Log("기본 아이템 지급 완료!");
        }
        else
        {
            Debug.Log("이미 기본 아이템이 지급되었습니다.");
        }
    }

    public void ResetDefaultItemGive()
    {
        PlayerPrefs.DeleteKey("FirstItemGiven");
        PlayerPrefs.Save();
        Debug.Log("기본 아이템 지급 상태 초기화됨");
    }

    public void ResetAllData()
    {
        //스킬 초기화
        currentSkills.Clear();
        PlayerPrefs.DeleteKey("PlayerSkillList");

        //코인 초기화
        totalCoin = 0;
        PlayerPrefs.DeleteKey("TotalCoin");

        //아이템 초기화
        Items.Clear();
        PlayerPrefs.DeleteKey("ItemList");
        PlayerPrefs.DeleteKey("FirstItemGiven");

        //플레이어 레벨/경험치 초기화
        playerLevel = 1;
        playerCurExp = 0;
        playerMaxExp = 100;
        PlayerPrefs.DeleteKey("PlayerLevel");
        PlayerPrefs.DeleteKey("PlayerCurExp");
        PlayerPrefs.DeleteKey("PlayerMaxExp");

        //킬카운트 초기화
        stage1kill = 0;
        stage2kill = 0;
        PlayerPrefs.DeleteKey("BestTotalKill");

        //몬스터 스케일 초기화
        monsterHp = 20f;
        monsterSpawnTime = 0.05f;
        monsterMaxCount = 500;
        PlayerPrefs.DeleteKey("MonsterHp");
        PlayerPrefs.DeleteKey("MonsterSpawnTime");
        PlayerPrefs.DeleteKey("MonsterMaxCount");

        //저장
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Debug.Log("모든 데이터 초기화 완료!");
    }
}
