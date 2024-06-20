using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatTest : MonoBehaviour
{
    public TMP_InputField chatinput;
    // Start is called before the first frame update
    void Start()
    {
        //chatinput = GameObject.Find("Chat").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        CHat();
    }

    void CHat()
    {       
            // 인풋 필드가 비어 있지 않고 Enter 키를 누르면 메시지 전송
            if (!string.IsNullOrEmpty(chatinput.text) && Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("1번비활성");
                chatinput.DeactivateInputField();
                chatinput.OnDeselect(null);
                chatinput.text = string.Empty;
                return;
            }

            // 인풋 필드가 포커스를 받지 않은 상태에서 Enter 키를 누르면 인풋 필드 활성화 및 선택
            if (!chatinput.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("1번활성");
                chatinput.ActivateInputField();
                chatinput.Select();
            }

    }
}
