using UnityEngine;
using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule
{
    private bool IsIgnoredKey(KeyCode keyCode)
    {
        // 무시할 키 입력 정의
        return keyCode == KeyCode.W || keyCode == KeyCode.S || keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow;
    }

    public override void Process()
    {
        // 기본 입력 처리
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
        // 현재 입력 이벤트 처리
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode) && IsIgnoredKey(keyCode))
                {
                    // 무시할 키 입력을 감지하면 아무 작업도 수행하지 않음
                    return;
                }
            }
        }

        // 기본 처리 로직 호출
        ProcessMouseEvent();
    }
}
