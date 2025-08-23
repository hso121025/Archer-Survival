//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Firebase.Auth;
//using UnityEngine.UI;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using Firebase.Extensions;
//using UnityEngine.SocialPlatforms;
//using Firebase;

//public class FirebaseAuth_Mgr : MonoBehaviour
//{
//    private FirebaseAuth auth;
//    private FirebaseUser user;

//    public InputField Email;
//    public InputField Password;

//    private bool firebaseReady = false;

//    void Awake()
//    {
//        InitGPGS();
//        InitFirebase();
//    }
//    void InitGPGS()
//    {
//        var config = new PlayGamesClientConfiguration.Builder()
//            .RequestEmail()
//            .RequestIdToken()
//            .Build();

//        PlayGamesPlatform.InitializeInstance(config);
//        PlayGamesPlatform.DebugLogEnabled = true;
//        PlayGamesPlatform.Activate();
//    }

//    void InitFirebase()
//    {
//        FirebaseApp.CheckAndFixDependenciesAsync()
//            .ContinueWithOnMainThread(task =>
//            {
//                if (task.Result == DependencyStatus.Available)
//                {
//                    auth = FirebaseAuth.DefaultInstance;
//                    firebaseReady = true;
//                    Debug.Log("[Firebase] �ʱ�ȭ ����");
//                }
//                else
//                {
//                    Debug.LogError("[Firebase] �ʱ�ȭ ����: " + task.Result);
//                }
//            });
//    }

//    public void TryGLogin()
//    {
//        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, status =>
//        {
//            if (status == SignInStatus.Success)
//            {
//                Debug.Log("[GPGS] �α��� ����");
//                StartCoroutine(TryFBLogin());
//            }
//            else
//            {
//                Debug.LogError("[GPGS] �α��� ����: " + status);
//            }
//        });
//    }

//    public void TryFBCreate()
//    {
//        if (!firebaseReady) { Debug.LogError("[Firebase] ���� �ʱ�ȭ ��"); return; }
//        if (string.IsNullOrEmpty(Email.text) || string.IsNullOrEmpty(Password.text))
//        {
//            Debug.LogError("[Firebase] �̸���/��й�ȣ�� ��� ����");
//            return;
//        }

//        auth.CreateUserWithEmailAndPasswordAsync(Email.text, Password.text)
//            .ContinueWithOnMainThread(task =>
//            {
//                if (task.IsCanceled)
//                {
//                    Debug.LogError("[Firebase] ȸ������ ���");
//                    return;
//                }
//                if (task.IsFaulted)
//                {
//                    Debug.LogError("[Firebase] ȸ������ ����: " + task.Exception);
//                    return; // �� ���� ��ȯ
//                }

//                FirebaseUser newUser = task.Result.User;
//                Debug.Log("[Firebase] ȸ������ ����: " + newUser.UserId);
//            });
//    }

//    IEnumerator TryFBLogin()
//    {
//        if (!firebaseReady)
//        {
//            Debug.LogError("[Firebase] ���� �ʱ�ȭ ��");
//            yield break;
//        }

//        var localUser = (PlayGamesLocalUser)Social.localUser;

//        // ID ��ū�� �غ�� ������ ���
//        while (string.IsNullOrEmpty(localUser.GetIdToken()))
//        {
//            yield return null;
//        }

//        string idToken = localUser.GetIdToken();

//        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

//        auth.SignInWithCredentialAsync(credential)
//            .ContinueWithOnMainThread(task =>
//            {
//                if (task.IsCanceled)
//                {
//                    Debug.LogError("[Firebase] ���� �ڰ����� �α��� ���");
//                    return;
//                }
//                if (task.IsFaulted)
//                {
//                    Debug.LogError("[Firebase] ���� �ڰ����� �α��� ����: " + task.Exception);
//                    return;
//                }

//                user = task.Result;
//                Debug.Log("[Firebase] ���� �ڰ����� �α��� ����: " + user.UserId);
//            });
//    }
//}

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
        //�� �̻� config �ʿ� ����
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
                Debug.Log("[Firebase] �ʱ�ȭ ����");
            }
            else
            {
                Debug.LogError("[Firebase] �ʱ�ȭ ����: " + task.Result);
            }
        });
    }

    public void TryGLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("[GPGS] �α��� ����");
                LinkWithFirebase();
                LogIn_UI.SetActive(false);
                Play_UI.SetActive(true);
            }
            else
            {
                Debug.LogError("[GPGS] �α��� ����");
            }
        });
    }

    // Firebase�� ����
    private void LinkWithFirebase()
    {
        if (!firebaseReady) { Debug.LogError("[Firebase] ���� �ʱ�ȭ ��"); return; }

        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            if (string.IsNullOrEmpty(code))
            {
                Debug.LogError("[GPGS] ServerAuthCode ����");
                return;
            }

            Debug.Log("[GPGS] ServerAuthCode: " + code);

            Credential credential = PlayGamesAuthProvider.GetCredential(code);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("[Firebase] �α��� ����: " + task.Exception);
                }
                else
                {
                    user = task.Result;
                    Debug.Log("[Firebase] �α��� ����: " + user.UserId);
                    LogIn_UI.SetActive(false);
                    Play_UI.SetActive(true);
                }
            });
        });
    }

    // �̸��� ȸ������
    public void TryFBLogin()
    {
        if (!firebaseReady) { Debug.LogError("[Firebase] ���� �ʱ�ȭ ��"); return; }

        auth.SignInWithEmailAndPasswordAsync(Email.text, Password.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("[Firebase] �α��� ���");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("[Firebase] �α��� ����: " + task.Exception);
                    return;
                }

                FirebaseUser loginUser = task.Result.User;
                Debug.Log("[Firebase] �α��� ����: " + loginUser.UserId);
                LogIn_UI.SetActive(false);
                Play_UI.SetActive(true);
            });
    }
}

