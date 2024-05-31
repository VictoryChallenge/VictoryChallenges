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

            c_playerNameInput.onValueChanged.AddListener(delegate { OnPlayerInputValueChange(c_playerNameInput); });
            f_playerNameInput.onValueChanged.AddListener(delegate { OnPlayerInputValueChange(f_playerNameInput); });
        }

        private void InitializePlayerName(TMP_InputField inputField)
        {
            if (PlayerPrefs.HasKey("username"))
            {
                string username = PlayerPrefs.GetString("username");
                inputField.text = username;
                PhotonNetwork.NickName = username;
            }
            else
            {
                string randomname = "Player " + Random.Range(0, 10000).ToString("0000");
                inputField.text = randomname;
                PhotonNetwork.NickName = randomname;
                PlayerPrefs.SetString("username", randomname);
            }
        }

        public void OnPlayerInputValueChange(TMP_InputField inputField)
        {
            string newUsername = inputField.text;
            PhotonNetwork.NickName = newUsername;
            PlayerPrefs.SetString("username", newUsername);
        }
    }
}
