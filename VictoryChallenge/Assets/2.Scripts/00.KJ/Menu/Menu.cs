using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;      // �޴� �̸�
    public bool open;            // Open bool ��

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
