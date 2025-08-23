using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScene_Mgr : MonoBehaviour
{
    public Button Stage01_Btn;
    public Button Stage02_Btn;
    public Button Boss_Btn;
    public Button Reste_Btn;
    public Button Back_Btn;
    public Text Cointext;


    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        if (Stage01_Btn != null)
            Stage01_Btn.onClick.AddListener(() => Stage01BtnClick());

        if (Stage02_Btn != null)
            Stage02_Btn.onClick.AddListener(() => Stage02BtnClick());

        if (Boss_Btn != null)
            Boss_Btn.onClick.AddListener(() => BossBtnClick());

        if (Reste_Btn != null)
            Reste_Btn.onClick.AddListener(() => ResetBtnClick());

        if (Back_Btn != null)
            Back_Btn.onClick.AddListener(() => BackBtnClick());

        Cointext.text = GlobalValue.Inst.totalCoin.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Stage01BtnClick()
    {
        SceneManager.LoadScene("00.UIScene");
        SceneManager.LoadScene("02.Stage1", LoadSceneMode.Additive);
        GlobalValue.Inst.GiveDefaultItemsOnce();
        GlobalValue.Inst.ClearSkills();



    }

    void Stage02BtnClick()
    {
        SceneManager.LoadScene("00.UIScene");
        SceneManager.LoadScene("03.Stage2", LoadSceneMode.Additive);
        GlobalValue.Inst.GiveDefaultItemsOnce();
        GlobalValue.Inst.ClearSkills();
    }

    void BossBtnClick()
    {
        SceneManager.LoadScene("00.UIScene");
        SceneManager.LoadScene("04.Boss", LoadSceneMode.Additive);
        GlobalValue.Inst.GiveDefaultItemsOnce();
        GlobalValue.Inst.ClearSkills();
    }

    void ResetBtnClick()
    {
        GlobalValue.Inst.ResetAllData();
    }

    void BackBtnClick()
    {
        SceneManager.LoadScene("06.TitleScene");
        GameMgr.Inst.ResetGame();
        Debug.Log("LobbyScene " + GlobalValue.Inst.SceneName);
    }
}
