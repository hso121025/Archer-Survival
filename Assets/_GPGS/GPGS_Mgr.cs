using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
public class GPGS_Mgr : MonoBehaviour
{
    private int killCount = 0;

    public GameObject LogIn_UI;
    public GameObject Play_UI;

    public static GPGS_Mgr Inst;

    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

        // Start is called before the first frame update
        void Start()
    {
        PlayGamesPlatform.Activate();
        GPGS_LogIn();

        LogIn_UI.SetActive(true);
        Play_UI.SetActive(false);
    }

    public void GPGS_LogIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log("[GPGS] �α��� ���� : " +
                      PlayGamesPlatform.Instance.GetUserDisplayName() +
                      " / " +
                      PlayGamesPlatform.Instance.GetUserId());

            LogIn_UI.SetActive(false);
            Play_UI.SetActive(true);
        }
        else
        {
            Debug.LogError("[GPGS] �α��� ����");
        }
    }

    public void ShowLeaderBoardUI()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_kill_count);
    }

    public void AddKillCount()
    {
        killCount++;
        PlayGamesPlatform.Instance.ReportScore(killCount, GPGSIds.leaderboard_kill_count, (bool success) =>
        {
            Debug.Log("[GPGS] �������� ������Ʈ: " + success);
        });
    }

    public void ShowAchievementUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public void IncrementGPGSAchievement()
    {
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement__stage_01_clear, 1, (bool success) => { });
    }

    public void UnlockingGPGSAchevement()
    {
        PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement__stage_02_clear, (bool success) => { });
    }



    public void OnStage1Cleared()
    {
        PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement__stage_01_clear, success =>
        {
            Debug.Log("[GPGS] Stage 01 Clear Unlock: " + success);

            if (success)
            {
                // ���� �ܰ� ������ 'Reveal' (�ʱ� Hidden�̸� ���� ó��)
                PlayGamesPlatform.Instance.RevealAchievement(GPGSIds.achievement__stage_02_clear, r =>
                {
                    Debug.Log("[GPGS] Reveal Stage 02 Achievement: " + r);
                });
            }
        });
    }

    public void OnStage2Cleared()
    {
        PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement__stage_02_clear, success =>
        {
            Debug.Log("[GPGS] Stage 02 Clear Unlock: " + success);
        });
    }

    public void ReportTotalKill()
    {
        long kills = GlobalValue.Inst.totalKill;  // GlobalValue���� ���� ų �� ��������
        PlayGamesPlatform.Instance.ReportScore(
            kills,
            GPGSIds.leaderboard_kill_count,       // �ֿܼ��� �߱޹��� �������� ID
            (bool success) =>
            {
                Debug.Log("[GPGS] TotalKill Reported: " + kills + " / success: " + success);
            });
    }
}

