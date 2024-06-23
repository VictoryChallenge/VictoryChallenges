using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Rank
{
    public class RankUI : MonoBehaviour
    {
        public TMP_Text rankText;
        private RankManagers _rankData;

        void Start()
        {
            _rankData = GetComponent<RankManagers>();
        }

        void Update()
        {
            rankText.text = "Rank : " + _rankData.Rank; 
        }
    }
}
