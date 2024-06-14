using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VictoryChallenge.Scripts.CL;

public class CustomButtonPopup : UI_Popup
{
    enum Buttons
    {
        Face,
        Head,
        Body,
        Hand,
        Toes,
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Face).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
    }

    public void OnButtonClicked(PointerEventData data, int a)
    {

        switch (a)
        {
            case 1:
                ClosePopupUI();
                Managers.UI.ShowPopupUI<UI_Popup>("CustomFacePopup");
                break;
            default:
                Debug.LogWarning("Unhandled action: " + a);
                break;
        }
    }
}
