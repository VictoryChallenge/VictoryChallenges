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
            // ��ǲ �ʵ尡 ��� ���� �ʰ� Enter Ű�� ������ �޽��� ����
            if (!string.IsNullOrEmpty(chatinput.text) && Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("1����Ȱ��");
                chatinput.DeactivateInputField();
                chatinput.OnDeselect(null);
                chatinput.text = string.Empty;
                return;
            }

            // ��ǲ �ʵ尡 ��Ŀ���� ���� ���� ���¿��� Enter Ű�� ������ ��ǲ �ʵ� Ȱ��ȭ �� ����
            if (!chatinput.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("1��Ȱ��");
                chatinput.ActivateInputField();
                chatinput.Select();
            }

    }
}
