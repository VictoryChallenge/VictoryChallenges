using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 데이터를 저장하는 방법
/// 1. 저장할 데이터가 존재
/// 2. 데이터를 제이슨으로 변환
/// 3. 제이슨을 외부에 저장

/// 데이터를 불러오는 방법
/// 1. 외부에 저장된 제이슨을 가져옴
/// 2. 제이슨을 데이터 형태로 변환
/// 3. 불러온 데이터를 사용

namespace VictoryChallenge.Json.DataManage
{
    public class PlayerData
    {
        // 이름, 레벨, 코인, 착용 아이템
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
            // 싱글톤
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
            playerData.name = "테스트플레이어";
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}
