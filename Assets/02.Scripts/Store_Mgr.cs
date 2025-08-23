using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public static Store_Mgr Inst;

    public Button Back_Btn;
    public Button Item1_Btn;
    public Button Item2_Btn;
    public Button Item3_Btn;

    public Text Cointext;
    public Text HeartText;
    public Text BombText;
    public Text MagnetText;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cointext.text = GlobalValue.Inst.totalCoin.ToString();

        if (Back_Btn != null)
            Back_Btn.onClick.AddListener(() => BackBtnClick());

        if (Item1_Btn != null)
            Item1_Btn.onClick.AddListener(() => BuyItem(1, 50));

        if (Item2_Btn != null)
            Item2_Btn.onClick.AddListener(() => BuyItem(2, 50));

        if (Item3_Btn != null)
            Item3_Btn.onClick.AddListener(() => BuyItem(3, 50));

        UpdateItemText();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void BackBtnClick()
    {
        SceneManager.LoadScene("06.TitleScene");
    }

    public void BuyItem(int itemID, int cost)
    {
        if (GlobalValue.Inst.totalCoin >= cost)
        {
            GlobalValue.Inst.totalCoin -= cost;
            GlobalValue.Inst.SaveCoin();
            GlobalValue.Inst.AddItem(itemID);
            Cointext.text = GlobalValue.Inst.totalCoin.ToString();

            UpdateItemText();
        }
    }
    public void UpdateItemText()
    {
        int HeartCount = GlobalValue.Inst.Items.FindAll(id => id == 1).Count;
        int BomgCount = GlobalValue.Inst.Items.FindAll(id => id == 2).Count;
        int MagnetCount = GlobalValue.Inst.Items.FindAll(id => id == 3).Count;

        HeartText.text = "x " + HeartCount;
        BombText.text = "x " + BomgCount;
        MagnetText.text = "x " + MagnetCount;
    }
}
