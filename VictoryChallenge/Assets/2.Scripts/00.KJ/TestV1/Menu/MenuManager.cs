using UnityEngine;

namespace VictoryChallenge.KJ.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;     // �̱���

        [SerializeField] Menu[] menus;          // �޴� ����Ʈ

        void Awake()
        {
            Instance = this;
        }

        public void OpenMenu(string menuName)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].menuName == menuName)
                {
                    menus[i].Open();
                }
                else if (menus[i].open)
                {
                    CloseMenu(menus[i]);
                }
            }
        }

        public void OpenMenu(Menu menu)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].open)
                {
                    CloseMenu(menus[i]);
                }
            }

            if (menu != null)
            {
                menu.Open();
            }
        }

        public void CloseMenu(Menu menu)
        {
            if (menu != null)
            {
                menu.Close();
            }
        }
    }
}
