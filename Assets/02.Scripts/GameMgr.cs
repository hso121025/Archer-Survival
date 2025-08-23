using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    [HideInInspector] public PlayerCtrl Player = null;
    [HideInInspector] public Item_Ctrl Item = null;
    [HideInInspector] public Monster_Ctrl Mons = null;

    public string SceneName;

    public Button Back_Btn;

    public GameObject TimerMain;

    int Level = 1;
    int curExp;
    float MaxExp = 100;
    public Image ExpBar;
    public Text LevelText;

    public Text CoinText;

    public Text KillCountText;
    public int KillCount = 0;

    public Text TimerText;
    float Timer = 300;    
    float Resettimer = 300; 

    public Text HeartText;
    public Text BombText;
    public Text MagnetText;

    public Button Heart_Btn;
    public Button Bomb_Btn;
    public Button Magnet_Btn;
    public Button RePlay_Btn;

    public Image PlayerDiePanel;

    public GameObject Bomb_Panel;

    public Text Stage1Kill;// 킬카운트 표시 해야함
    public Text Stage2Kill;
    public Text TotalKill;
    public Text BestKill;

    //--- 싱글턴 패턴을 위한 인스턴스 변수 선언
    public static GameMgr Inst = null;

    void Awake()
    {
        Inst = this;

        if (Back_Btn != null)
            Back_Btn.onClick.AddListener(() => BackBtnClick());

        if(RePlay_Btn != null)
            RePlay_Btn.onClick.AddListener(() => StageReset());

        Application.targetFrameRate = 60;    //프레임 제한
        QualitySettings.vSyncCount = 0;


    }
    //--- 싱글턴 패턴을 위한 인스턴스 변수 선언


    // Start is called before the first frame update
    void Start()
    {
        SceneName = GlobalValue.Inst.SceneName;
        Debug.Log(SceneName);

        Player = GameObject.FindObjectOfType<PlayerCtrl>();
        Item = GameObject.FindObjectOfType<Item_Ctrl>();
        Mons = GameObject.FindAnyObjectByType<Monster_Ctrl>();

        Level = GlobalValue.Inst.playerLevel;
        curExp = GlobalValue.Inst.playerCurExp;
        MaxExp = GlobalValue.Inst.playerMaxExp;

        LevelText.text = "Lv. " + Level;
        ExpBar.fillAmount = (float)curExp / MaxExp;

        CoinText.text = GlobalValue.Inst.totalCoin.ToString();

        if (Heart_Btn != null)
            Heart_Btn.onClick.AddListener(UseHeart);

        if (Bomb_Btn != null)
            Bomb_Btn.onClick.AddListener(UseBomb);

        if (Magnet_Btn != null)
            Magnet_Btn.onClick.AddListener(UseMagnet);

        ItemTextUpdate();
    }

    void Update()
    {
        ResetTimer();
    }

    public void GetExp(int EXP)
    {
        curExp += EXP;

        while (curExp >= MaxExp)
        {
            curExp -= (int)MaxExp;
            LevelUp();
        }

        ExpBar.fillAmount = (float)curExp / MaxExp;

        GlobalValue.Inst.playerLevel = Level;
        GlobalValue.Inst.playerCurExp = curExp;
        GlobalValue.Inst.playerMaxExp = MaxExp;
        GlobalValue.Inst.SaveExpData();
    }

    public void GetCoin(int Coin)
    {
        GlobalValue.Inst.AddCoin(Coin);
        CoinText.text = GlobalValue.Inst.totalCoin.ToString();
    }

    void LevelUp()
    {
        Level++;
        MaxExp *= 1.5f;
        LevelText.text = "Lv . " + Level;

        Skill_Mgr.Inst.SelectSkill();
    }

    public void Kill()
    {
        KillCount++;
        KillCountText.text = KillCount.ToString();

        if (SceneName == "02.Stage1")
            GlobalValue.Inst.AddStage1Kill();
        else if (SceneName == "03.Stage2")
            GlobalValue.Inst.AddStage2Kill();
    }

    void ItemTextUpdate()
    {
        int item1Count = GlobalValue.Inst.Items.FindAll(id => id == 1).Count;
        int item2Count = GlobalValue.Inst.Items.FindAll(id => id == 2).Count;
        int item3Count = GlobalValue.Inst.Items.FindAll(id => id == 3).Count;

        HeartText.text = "x " + item1Count;
        BombText.text = "x " + item2Count;
        MagnetText.text = "x " + item3Count;
    }

    void ResetTimer()
    {
        if (SceneName == "02.Stage1")
        {
            if (0.0f <= Timer)
            {
                Timer -= Time.deltaTime;

                int minutes = (int)(Timer / 60);
                int seconds = (int)(Timer % 60);

                TimerText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                GPGS_Mgr.Inst.OnStage1Cleared();
                Timer = Resettimer;
                SceneManager.LoadScene("00.UIScene");
                SceneManager.LoadScene("03.Stage2", LoadSceneMode.Additive);
            }
        }
        else if (SceneName == "03.Stage2")
        {
            if (0.0f <= Timer)
            {
                Timer -= Time.deltaTime;

                int minutes = (int)(Timer / 60);
                int seconds = (int)(Timer % 60);

                TimerText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                GPGS_Mgr.Inst.OnStage2Cleared();
                Timer = Resettimer;
                SceneManager.LoadScene("00.UIScene");
                SceneManager.LoadScene("04.Boss", LoadSceneMode.Additive);
            }
        }
        else
        {
            TimerMain.SetActive(false);
        }

    }

    void UseHeart()
    {
        if (GlobalValue.Inst.Items.Contains(1))
        {
            Player.Hp += 50;
            if (Player.Hp >= 150)
                Player.Hp = 150;
            Player.UpdateHpUI();
            GlobalValue.Inst.Items.Remove(1);
            GlobalValue.Inst.SaveItems();
            ItemTextUpdate();
        }
        Debug.Log("현재 Player HP: " + Player.Hp);
    }

    void UseBomb()
    {
        if (GlobalValue.Inst.Items.Contains(2)) // 폭탄이 있으면
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, 500.0f);
            foreach (var coll in colls)
            {
                var mon = coll.GetComponent<Monster_Ctrl>();
                var turtleShell = coll.GetComponent<TurtleShell_Ctrl>();

                if (mon != null || turtleShell != null)
                {
                    MonsterPool_Mgr.Inst.ReturnToPool(coll.gameObject);
                    MonsterPool_Mgr.Inst.ReturnToPool(coll.gameObject);
                }
            }

            StartCoroutine(BombEffect());
            GlobalValue.Inst.Items.Remove(2);
            GlobalValue.Inst.SaveItems();
            ItemTextUpdate();
        }
    }

    void UseMagnet()
    {
        if (GlobalValue.Inst.Items.Contains(3)) // 자석이 있으면
        {
            foreach (var item in FindObjectsOfType<Item_Ctrl>())
            {
                if (item.itemType == ItemType.EXP)
                {
                    item.GetComponent<CapsuleCollider>().isTrigger = true;
                    item.StartMoveToPlayer(Player.transform);
                }
            }

            GlobalValue.Inst.Items.Remove(3);
            GlobalValue.Inst.SaveItems();
            ItemTextUpdate();
        }
    }

    public void PlayerDie()
    {
        Time.timeScale = 0;
        PlayerDiePanel.gameObject.SetActive(true);
        Back_Btn.gameObject.SetActive(false);
        JoyStick_Mgr.Inst.JoystickPickPanel.gameObject.SetActive(false);

        GlobalValue.Inst.SaveBestTotalKillNew();

        int s1 = GlobalValue.Inst.stage1kill;
        int s2 = GlobalValue.Inst.stage2kill;
        int total = GlobalValue.Inst.totalKill;
        int best = GlobalValue.Inst.GetBestTotalKill();

        Stage1Kill.text = s1.ToString();
        Stage2Kill.text = s2.ToString();
        TotalKill.text = total.ToString();
        BestKill.text = best.ToString();
    }

    void StageReset()
    {
        Time.timeScale = 1;
        Back_Btn.gameObject.SetActive(true) ;
        SceneManager.LoadScene("01.LobbyScene");
        JoyStick_Mgr.Inst.JoystickPickPanel.gameObject.SetActive(true);

        ResetGame();
    }

    public void ResetGame()
    {
        GlobalValue.Inst.ResetKillCounts();
        GlobalValue.Inst.ResetExpData();
        GlobalValue.Inst.ResetMonsterStats();
    }

    void BackBtnClick()
    {
        SceneManager.LoadScene("01.LobbyScene");
        GameMgr.Inst.ResetGame();
    }

    public IEnumerator BombEffect()
    {
        Bomb_Panel.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        Bomb_Panel.gameObject.SetActive(false);
    }
}


