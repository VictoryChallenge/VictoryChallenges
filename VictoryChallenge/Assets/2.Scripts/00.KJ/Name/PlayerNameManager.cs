using Photon.Pun;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Name
{
    public class PlayerNameManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField c_playerNameInput;        // ���� �̸� ���� �Է� �ʵ� (Ŀ���� ��)
        [SerializeField] TMP_InputField f_playerNameInput;        // ���� �̸� ���� �Է� �ʵ� (�� ã��)

        void Start()
        {
            InitializePlayerName(c_playerNameInput);
            InitializePlayerName(f_playerNameInput);
        }

        private void InitializePlayerName(TMP_InputField inputField)
        {
            if (PlayerPrefs.HasKey("username"))
            {
                inputField.text = PlayerPrefs.GetString("username");
                PhotonNetwork.NickName = PlayerPrefs.GetString("username");
            }
            else
            {
                inputField.text = "Player " + Random.Range(0, 10000).ToString("0000");
                OnPlayerInputValueChange(inputField);
            }
        }

        public void OnPlayerInputValueChange(TMP_InputField inputField)
        {
            PhotonNetwork.NickName = inputField.text;
            PlayerPrefs.GetString("username", inputField.text);
        }
    }
}
