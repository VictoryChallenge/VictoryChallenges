using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// �����͸� �����ϴ� ���
/// 1. ������ �����Ͱ� ����
/// 2. �����͸� ���̽����� ��ȯ
/// 3. ���̽��� �ܺο� ����

/// �����͸� �ҷ����� ���
/// 1. �ܺο� ����� ���̽��� ������
/// 2. ���̽��� ������ ���·� ��ȯ
/// 3. �ҷ��� �����͸� ���

namespace VictoryChallenge.Json.DataManage
{
    public class PlayerData
    {
        // �̸�, ����, ����, ���� ������
        public string name;
        public int level;
        public int coin;
        public int item;
    }

    public class JsonDataManager : MonoBehaviour
    {
        public static JsonDataManager instance;

        PlayerData playerData = new PlayerData();

        private void Awake()
        {
            // �̱���
            if (instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(instance.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            playerData.name = "�׽�Ʈ�÷��̾�";
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}
