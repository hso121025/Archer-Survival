using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class FirebaseAuth_Mgr : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;

    public InputField Email;
    public InputField Password;

    public GameObject LogIn_UI;
    public GameObject Play_UI;

    private bool firebaseReady = false;

    void Awake()
    {
        InitGPGS();
        InitFirebase();

        LogIn_UI.SetActive(true);
        Play_UI.SetActive(false);
    }

    void InitGPGS()
    {
        //더 이상 config 필요 없음
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                firebaseReady = true;
                Debug.Log("[Firebase] 초기화 성공");
            }
            else
            {
                Debug.LogError("[Firebase] 초기화 실패: " + task.Result);
            }
        });
    }

    public void TryGLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("[GPGS] 로그인 성공");
                LinkWithFirebase();
                LogIn_UI.SetActive(false);
                Play_UI.SetActive(true);
            }
            else
            {
                Debug.LogError("[GPGS] 로그인 실패");
            }
        });
    }

    // Firebase와 연동
    private void LinkWithFirebase()
    {
        if (!firebaseReady) { Debug.LogError("[Firebase] 아직 초기화 전"); return; }

        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            if (string.IsNullOrEmpty(code))
            {
                Debug.LogError("[GPGS] ServerAuthCode 없음");
                return;
            }

            Debug.Log("[GPGS] ServerAuthCode: " + code);

            Credential credential = PlayGamesAuthProvider.GetCredential(code);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("[Firebase] 로그인 실패: " + task.Exception);
                }
                else
                {
                    user = task.Result;
                    Debug.Log("[Firebase] 로그인 성공: " + user.UserId);
                    LogIn_UI.SetActive(false);
                    Play_UI.SetActive(true);
                }
            });
        });
    }

    // 이메일 회원가입
    public void TryFBLogin()
    {
        if (!firebaseReady) { Debug.LogError("[Firebase] 아직 초기화 전"); return; }

        auth.SignInWithEmailAndPasswordAsync(Email.text, Password.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("[Firebase] 로그인 취소");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("[Firebase] 로그인 실패: " + task.Exception);
                    return;
                }

                FirebaseUser loginUser = task.Result.User;
                Debug.Log("[Firebase] 로그인 성공: " + loginUser.UserId);
                LogIn_UI.SetActive(false);
                Play_UI.SetActive(true);
            });
    }
}

