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
            Debug.Log("[GPGS] 로그인 성공 : " +
                      PlayGamesPlatform.Instance.GetUserDisplayName() +
                      " / " +
                      PlayGamesPlatform.Instance.GetUserId());

            LogIn_UI.SetActive(false);
            Play_UI.SetActive(true);
        }
        else
        {
            Debug.LogError("[GPGS] 로그인 실패");
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
            Debug.Log("[GPGS] 리더보드 업데이트: " + success);
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
}

