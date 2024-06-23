using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.KJ.Rank
{
    public class RankManagers : MonoBehaviour
    {
        public List<RankMovement> players = new List<RankMovement>();

        public void RegisterPlayer(RankMovement player)
        {
            players.Add(player);
        }

        public int Rank { get; set; }

        void Update()
        {
            CalculateRankAndDistributeScores();
        }

        void CalculateRankAndDistributeScores()
        {
            Dictionary<RankMovement, float> playerDistances = new Dictionary<RankMovement, float>();
            foreach (var player in players)
            {
                playerDistances[player] = player.transform.position.z;
            }

            List<KeyValuePair<RankMovement, float>> sortedPlayers = new List<KeyValuePair<RankMovement, float>>(playerDistances);
            sortedPlayers.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            int rank = 1;
            foreach (var playerPair in sortedPlayers)
            {
                RankMovement player = playerPair.Key;
                Rank = rank;
                rank++;
            }
        }
    }

}