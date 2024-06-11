using Photon.Pun;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Name
{
    public class NameInput : MonoBehaviour
    {
        [SerializeField] TMP_InputField nickNameInputField;     // �г��� �Է� �ʵ�

        void Start()
        {
            InitializeNickname(nickNameInputField);
            nickNameInputField.onValueChanged.AddListener(delegate { OnNicknameValueChanged(nickNameInputField); });
        }

        private void InitializeNickname(TMP_InputField inputfield)
        {
            if (PlayerPrefs.HasKey("username"))
            {
                string username = PlayerPrefs.GetString("username");
                nickNameInputField.text = username;
                PhotonNetwork.NickName = username;
            }
            else
            {
                Debug.LogError("�г����� �Է��ϼ���.");
            }
        }

        public void OnNicknameValueChanged(TMP_InputField inputfield)
        {
            string newUserName = nickNameInputField.text;
            PhotonNetwork.NickName = newUserName;
            PlayerPrefs.SetString("username", newUserName );
        }
    }
}
