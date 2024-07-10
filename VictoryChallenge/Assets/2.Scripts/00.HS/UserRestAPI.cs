using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.HS
{
    [Serializable]
    public class UserRestAPI
    {
        public string userName;
        public int userScore;
        public string localId;

        public UserRestAPI()
        {
            userName = PlayerScoreRestAPI.playerName;
            userScore = PlayerScoreRestAPI.playerScore;
            localId = PlayerScoreRestAPI.localId;
        }
    }

}
