using Photon.Pun;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Name
{
    public class NameInput : MonoBehaviour
    {
        [SerializeField] TMP_InputField nickNameInputField;     // 닉네임 입력 필드

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
                Debug.LogError("닉네임을 입력하세요.");
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
