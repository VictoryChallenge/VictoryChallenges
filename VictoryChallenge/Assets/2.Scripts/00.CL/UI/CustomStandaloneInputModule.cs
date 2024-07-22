using UnityEngine;
using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule
{
    private bool IsIgnoredKey(KeyCode keyCode)
    {
        // ������ Ű �Է� ����
        return keyCode == KeyCode.W || keyCode == KeyCode.S || keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow;
    }

    public override void Process()
    {
        // �⺻ �Է� ó��
        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }

        // Custom key processing
        ProcessCustomInput();
    }

    private void ProcessCustomInput()
    {
        // ���� �Է� �̺�Ʈ ó��
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode) && IsIgnoredKey(keyCode))
                {
                    // ������ Ű �Է��� �����ϸ� �ƹ� �۾��� �������� ����
                    return;
                }
            }
        }

        // �⺻ ó�� ���� ȣ��
        ProcessMouseEvent();
    }
}
