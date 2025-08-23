using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Clear_Mgr : MonoBehaviour
{
    public Animator animator;

    public Text Stage1Kill;
    public Text Stage2Kill;
    public Text TotalKill;
    public Text Coin;
    public Text BestKill;

    public Text F2_Text;
    public Text FB_Text;
    public Text FF_Text;
    public Text RL_Text;
    public Text RL2_Text;
    public Text ATKUp_Text;
    public Text SpeedUp_Text;

    int totalKill;

    public Button Lobby_Btn;

    // Start is called before the first frame update
    void Start()
    {
        int stage1Kills = GlobalValue.Inst.stage1kill;
        int stage2Kills = GlobalValue.Inst.stage2kill;
        totalKill = GlobalValue.Inst.totalKill;

        int bestkill = GlobalValue.Inst.GetBestTotalKill();

        GlobalValue.Inst.SaveBestTotalKillNew();

        ShowSkillCounts();

        Stage1Kill.text = stage1Kills.ToString();
        Stage2Kill.text = stage2Kills.ToString();
        TotalKill.text = totalKill.ToString();
        BestKill.text = bestkill.ToString();

        Coin.text = GlobalValue.Inst.totalCoin.ToString();

        if (Lobby_Btn != null)
            Lobby_Btn.onClick.AddListener(() => LobbyBtnClick());
    }


    void LobbyBtnClick()
    {
        SceneManager.LoadScene("01.LobbyScene");
        GameMgr.Inst.ResetGame();
    }
    void ShowSkillCounts()
    {
        F2_Text.text = GetSkillCount(0).ToString();
        FB_Text.text = GetSkillCount(1).ToString();
        FF_Text.text = GetSkillCount(2).ToString();
        RL_Text.text = GetSkillCount(3).ToString();
        RL2_Text.text = GetSkillCount(4).ToString();
        ATKUp_Text.text = GetSkillCount(5).ToString();
        SpeedUp_Text.text = GetSkillCount(6).ToString();
    }

    int GetSkillCount(int skillID)
    {
        return GlobalValue.Inst.currentSkills.FindAll(x => x == skillID).Count;
    }
}
