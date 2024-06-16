using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<User> users = new List<User>();
}

[System.Serializable]
public class User
{
    public string uid;
    public List<UserInformation> userinformations = new List<UserInformation>();
    public int gold;
    // 캐릭터 관련
}

[System.Serializable]
public class UserInformation
{
    public int win;
    public int lose;
}