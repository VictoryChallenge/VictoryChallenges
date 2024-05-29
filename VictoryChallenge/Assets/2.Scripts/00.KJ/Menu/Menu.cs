using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;      // 메뉴 이름
    public bool open;            // Open bool 값

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
