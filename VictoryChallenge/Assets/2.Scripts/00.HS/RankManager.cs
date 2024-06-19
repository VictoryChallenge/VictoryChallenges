using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace VictoryChallenge.Scripts.HS
{
    public class RankManager : SingletonLazy<RankManager>
    {
        // 플레이어 데이터 담기
        // Dictionary<shortUID, rank>
        public Dictionary<string, int> playerDatas = new Dictionary<string, int>();

        public void Register(string shortUID, int rank)
        {
            playerDatas.Add(shortUID, rank);
        }

        public void SortRank()
        {
            playerDatas = playerDatas.OrderBy(x => x.Value).ToDictionary(x =>  x.Key, x => x.Value);
        }
    }
}
