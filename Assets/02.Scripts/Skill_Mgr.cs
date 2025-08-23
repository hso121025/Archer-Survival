using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Skill_Mgr : MonoBehaviour
{
    public Button[] Skill_Btn;
    public GameObject LevelUpPanel;

    public static Skill_Mgr Inst = null;

    int RanSkill;

    private void Awake()
    {
        Inst = this;
    }

    public void SelectSkill()
    {
        JoyStick_Mgr.Inst.JoystickPickPanel.SetActive(false);

        GameMgr.Inst.Heart_Btn.gameObject.SetActive(false);
        GameMgr.Inst.Bomb_Btn.gameObject.SetActive(false);
        GameMgr.Inst.Magnet_Btn.gameObject.SetActive(false);

        Time.timeScale = 0.0f;
        LevelUpPanel.SetActive(true);

        foreach (Button btn in Skill_Btn)
        {
            btn.gameObject.SetActive(false);
            btn.onClick.RemoveAllListeners(); // 기존 연결 제거
        }

        List<int> selected = new List<int>();

        for (int i = 0; i < 3;)
        {
            RanSkill = Random.Range(0, 7);

            if (selected.Contains(RanSkill))
                continue;

            selected.Add(RanSkill);
            Skill_Btn[RanSkill].gameObject.SetActive(true);

            int index = RanSkill;
            Skill_Btn[RanSkill].onClick.AddListener(() => { GlobalValue.Inst.AddSkill(index); SkillUpdate(index);});


            i++;
        }
        
    }

    public void SkillUpdate(int skillIndex)
    {
        Time.timeScale = 1.0f;
        LevelUpPanel.SetActive(false);

        var player = GameMgr.Inst.Player.GetComponent<PlayerCtrl>();

        switch (skillIndex)
        {
            case 0: //FireBall_F2
                player.Skill_F2 = true;

                if (player.FireBall_F2_Level < 1)
                    player.FireBall_F2_Level = 1;
                else
                    player.FireBall_F2_Level++;

                break;

            case 1: //FireBall_FB
                player.Skill_FB = true;

                if (player.FireBall_FB_Level < 1)
                    player.FireBall_FB_Level = 1;
                else
                    player.FireBall_FB_Level++;

                break;

            case 2: //FireBall_FF
                player.Skill_FF = true;

                if (player.FireBall_FF_Level < 1)
                    player.FireBall_FF_Level = 1;
                else
                    player.FireBall_FF_Level++;

                break;

            case 3: //FireBall_RL
                player.Skill_RL = true;

                if (player.FireBall_RL_Level < 1)
                    player.FireBall_RL_Level = 1;
                else
                    player.FireBall_RL_Level++;

                break;

            case 4: //FireBall_RL2
                player.Skill_RL2 = true;

                if (player.FireBall_RL2_Level < 1)
                    player.FireBall_RL2_Level = 1;
                else
                    player.FireBall_RL2_Level++;

                break;

            case 5: //SpeedUp
                player.MoveSpeed *= 1.1f; 
                break;

            case 6: //ATKUp
                player.BulletDamage *= 1.1f;
                break;
        }
        JoyStick_Mgr.Inst.JoystickPickPanel.SetActive(true);
        FlexibleJoy_Mgr.Inst.ResetJoy();

        GameMgr.Inst.Heart_Btn.gameObject.SetActive(true);
        GameMgr.Inst.Bomb_Btn.gameObject.SetActive(true);
        GameMgr.Inst.Magnet_Btn.gameObject.SetActive(true);
    }
}
