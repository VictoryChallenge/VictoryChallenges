using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using VictoryChallenge.KJ.Database;


namespace VictoryChallenge.Scripts.HS
{
    public class RankManager : SingletonLazy<RankManager>
    {
        // 플레이어 데이터 담기
        // Dictionary<shortUID, rank>
        public Dictionary<string, int> playerDatas = new Dictionary<string, int>();
        public Dictionary<string, int> playerScoreDatas = new Dictionary<string, int>();

        public enum Point
        {
            first =     10,
            second =    8,
            third =     6,
            fourth =    5,
            fifth =     4,
            sixth =     3,
            seventh =   2,
            eighth =    1,
        }

        public void Register(string shortUID, int rank = 0)
        {
            // 플레이어의 ShortUID와 Rank를 Dictionary에 저장
            playerDatas.Add(shortUID, rank);
            playerScoreDatas.Add(shortUID, rank);

            foreach (var item in playerDatas.Keys)
            {
                Debug.Log("KeyUser : " + item);
            }

            foreach (var item in playerDatas.Values)
            {
                Debug.Log("ValuesUser : " + item);
            }

            foreach (var item in playerScoreDatas.Keys)
            {
                Debug.Log("KeyScore : " + item);
            }

            foreach (var item in playerScoreDatas.Values)
            {
                Debug.Log("ValuesScore : " + item);
            }

        }

        public void SetRank(string shortUID, int rank)
        {
            // 플레이어의 ShortUID와 Rank를 저장
            playerDatas[shortUID] = rank;

            User user = DatabaseManager.Instance.gameData.users[shortUID];

            int score = RewardPoint(rank);

            // 순위에 맞는 점수, 골드 갱신
            user.score += score;

            playerScoreDatas[shortUID] = user.score;

            string userData = JsonUtility.ToJson(user);

            Debug.Log("userJsonData " + userData);

            DatabaseManager.Instance.WriteUserData(shortUID, userData);
        }

        public string SortRank()
        {
            playerScoreDatas = playerScoreDatas.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach(var item in playerScoreDatas.Keys)
            {
                Debug.Log("Key : " + item);
            }

            foreach(var item in playerScoreDatas.Values)
            {
                Debug.Log("Values : " + item);
            }


            DatabaseManager.Instance.ReadUserData("");

            return playerScoreDatas.First().Key.ToString();
        }

        public void ChooseWinner()
        {
            // 우승자
            string shortUID = SortRank();
            Debug.Log("Winnder ShortUID: " + shortUID);
            //User user = DatabaseManager.Instance.gameData.users[shortUID];

            
        }

        public int RewardPoint(int rank)
        {
            Point point;

            switch (rank)
            {
                case 1:
                    point = Point.first;
                    break;
                case 2:
                    point = Point.second;
                    break;
                case 3:
                    point = Point.third;
                    break;
                case 4:
                    point = Point.fourth;
                    break;
                case 5:
                    point = Point.fifth;
                    break;
                case 6:
                    point = Point.sixth;
                    break;
                case 7:
                    point = Point.seventh;
                    break;
                case 8:
                    point = Point.eighth;
                    break;
                default:
                    point = 0;
                    break;
            }

            Debug.Log("int point" + (int)point);
            return (int)point;
        }
    }
}