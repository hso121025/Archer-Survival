using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum JoyStickType
{
    Flexible = 0,
    FlexibleOnOff = 1
}

public class JoyStick_Mgr : MonoBehaviour
{
    [HideInInspector] public JoyStickType JoyStickType = JoyStickType.Flexible;

    public GameObject JoystickPickPanel = null;
    public GameObject JoySitck_Back = null;
    public Image JoySitck_Handle = null;

    //--- ╫л╠шео фпео
    public static JoyStick_Mgr Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- ╫л╠шео фпео

    // Start is called before the first frame update
    void Start()
    {
        if (JoystickPickPanel != null && JoystickPickPanel != null &&
            JoySitck_Handle != null &&
            JoystickPickPanel.activeSelf == true)
        {
            if (JoySitck_Back.activeSelf == true)
                JoyStickType = JoyStickType.Flexible;
            else
                JoyStickType = JoyStickType.FlexibleOnOff;

            JoySitck_Back.GetComponent<Image>().raycastTarget = false;
            JoySitck_Handle.raycastTarget = false;
        }
    }
}
