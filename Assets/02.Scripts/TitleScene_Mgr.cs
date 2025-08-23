using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene_Mgr : MonoBehaviour
{
    public Button Play_Btn;
    public Text Cointext;
    public Button Store_Btn;
    public Button Exit_Btn;

    // Start is called before the first frame update
    void Start()
    {
        if (Play_Btn != null)
            Play_Btn.onClick.AddListener(() => PlayBtnClick());

        if (Store_Btn != null)
            Store_Btn.onClick.AddListener(() => StoreBtnClick());

        if (Exit_Btn != null)
            Exit_Btn.onClick.AddListener(() => Application.Quit());

        Cointext.text = GlobalValue.Inst.totalCoin.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayBtnClick()
    {
        SceneManager.LoadScene("01.LobbyScene");
        GlobalValue.Inst.ClearSkills();
    }

    void StoreBtnClick()
    {
        SceneManager.LoadScene("07.StoreScene");
    }
}
