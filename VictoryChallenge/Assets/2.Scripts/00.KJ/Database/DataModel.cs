using System;
using System.Collections.Generic;

namespace VictoryChallenge.KJ.Database
{
    [System.Serializable]
    public class GameData
    {
        public Dictionary<string, User> users = new Dictionary<string, User>();
    }

    [System.Serializable]
    public class User
    {
        public string uid;
        public string shortUID;
        public string userName;
        public int gold;
        public int rank;

        public User() { }

        public User(string uid, string shortUID, string userName, int gold, int rank)
        {
            this.uid = uid;
            this.shortUID = shortUID;
            this.userName = userName;
            this.gold = gold;
            this.rank = rank;
        }
    }
}
