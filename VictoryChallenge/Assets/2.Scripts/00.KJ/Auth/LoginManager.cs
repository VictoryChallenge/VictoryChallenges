using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject _loginPanel;                            // �α��� �г�
    [SerializeField] private GameObject _registerPanel;                         // ȸ������ �г�

    public enum ActivePanel { Login, Register }
    public ActivePanel activePanel;
    public List<TMP_InputField> loginInputFields;
    public List<TMP_InputField> registerInputFields;

    private Stack<GameObject> _activePanels = new Stack<GameObject>();          // �� ����

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (activePanel)
            {
                case ActivePanel.Login:
                    NavigateThroughInputField(loginInputFields);
                    break;
                case ActivePanel.Register:
                    NavigateThroughInputField(registerInputFields);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitLastPanel();
        }
    }

    /// <summary>
    /// �α��� â ������
    /// </summary>
    public void ShowLoginPanel()
    {
        _loginPanel.SetActive(true);
        _activePanels.Push(_loginPanel);
    }

    /// <summary>
    /// ȸ������â ������
    /// </summary>
    public void ShowRegisterPanel()
    {
        _registerPanel.SetActive(true);
        _activePanels.Push(_registerPanel);
    }

    /// <summary>
    /// ���������� ���� UI â �ݱ�
    /// </summary>
    public void ExitLastPanel()
    {
        if (_activePanels.Count > 0)
        {
            GameObject lastPanel = _activePanels.Pop();
            lastPanel.SetActive(false);
        }
    }

    public void NavigateThroughInputField(List<TMP_InputField> inputFields)
    {
        for (int i = 0;  i < inputFields.Count; i++)
        {
            if (inputFields[i].isFocused)
            {
                int nextIndex = (i + 1) % inputFields.Count;
                inputFields[nextIndex].Select();
                inputFields[nextIndex].ActivateInputField();
                break;
            }
        }
    }

    public void SetActivePanel(ActivePanel panel)
    {
        activePanel = panel;
    }

    public void SetActivePanelToLogin()
    {
        SetActivePanel(ActivePanel.Login);
    }

    public void SetActivePanelToRegister()
    {
        SetActivePanel(ActivePanel.Register);
    }
}
