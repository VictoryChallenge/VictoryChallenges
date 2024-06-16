using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VictoryChallenge.Scripts.CL;

public class CustomFacePopup : UI_Popup
{
    enum Buttons
    {
        Eyes,
        Ears,
        Mouth,
        Back,
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Back).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
    }

    public void OnButtonClicked(PointerEventData data, int a)
    {

        switch (a)
        {
            case 1:
                ClosePopupUI();
                Managers.UI.ShowPopupUI<UI_Popup>("CustomButtonPopup");
                break;
            default:
                Debug.LogWarning("Unhandled action: " + a);
                break;
        }
    }
}
