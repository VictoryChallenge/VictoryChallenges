using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.HS
{
    public class PlayerScoreRestAPI : MonoBehaviour
    {
        public Text scoreText;
        public InputField getScoreText;

        public InputField userNameText;
        public InputField emailText;
        public InputField passwordText;

        private System.Random random = new System.Random();
        UserRestAPI user = new UserRestAPI();

        private string databaseURL = "https://unityrestapi-bbf08-default-rtdb.firebaseio.com/users/";
        private string AuthKey = "0pIJrDwDyNTsEk4irKkKcvpyTX6VcDhPulJFjSmJPl0llNatVd1KeWEA8BMZI2FsmgrE4G67cUXUbU1jZ104Cw";

        public static int playerScore;
        public static string playerName;

        private string idToken;
        public static string localId;

        void Start()
        {
            playerScore = random.Next(0, 101);
            scoreText.text = "Score " + playerScore;
        }

        public void OnSubmit()
        {
            //playerName = getScoreText.text;
            PostToDatabase();
        }

        public void OnGetScore()
        {
            RetrieveFromDatabase();
        }

        private void UpdateScore()
        {
            scoreText.text = "Score: " + user.userScore;
        }

        private void PostToDatabase(bool emptyScore = false)
        {
            UserRestAPI user = new UserRestAPI();
            if(emptyScore)
            {
                user.userScore = 0;
            }

            RestClient.Put(databaseURL + localId + ".json", user);
        }

        private void RetrieveFromDatabase()
        {
            RestClient.Get<UserRestAPI>(databaseURL + getScoreText.text + ".json")
                .Then(response =>
                {
                    user = response;
                    UpdateScore();
                });
        }

        public void SignUpUserButton()
        {
            SignUpUser(emailText.text, userNameText.text, passwordText.text);
        }

        public void SignInUserButton()
        {
            SignInUser(emailText.text, passwordText.text);
        }

        private void SignUpUser(string email, string username, string password)
        {
            string userData = "{\"email\":\""+ email + "\", \"password\":\""+ password + "\", \"returnSecureToken\":true}";
            RestClient.Post<SignResponse>("" + AuthKey, userData).Then(response =>
            {
                localId = response.localId;
                idToken = response.idToken;
                playerName = username;
                PostToDatabase(true);
            }).Catch(error =>
            {
                Debug.Log(error);
            });
        }

        private void SignInUser(string email, string password)
        {
            string userData = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\", \"returnSecureToken\":true}";
            RestClient.Post<SignResponse>("" + AuthKey, userData).Then(response =>
            {
                localId = response.localId;
                idToken = response.idToken;
                GetUserName();
            }).Catch(error =>
            {
                Debug.Log(error);
            });
        }

        private void GetUserName()
        {
            RestClient.Get<UserRestAPI>(databaseURL + localId + ".json")
                .Then(response =>
                {
                    response.userName = playerName;
                });
        }
    }
    
}
