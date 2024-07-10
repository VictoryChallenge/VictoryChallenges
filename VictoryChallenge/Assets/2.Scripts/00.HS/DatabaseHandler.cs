using Proyecto26;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace VictoryChallenge.Scripts.HS
{
    public class DatabaseHandler
    {
        private const string projectId = "unityrestapi-bbf08"; // You can find this in your Firebase project settings
        private static readonly string databaseURL = $"https://{projectId}-default-rtdb.firebaseio.com/";

        private static fsSerializer serializer = new fsSerializer(); 

        public delegate void PostUserCallback();
        public delegate void GetUserCallback(UserBlog user);
        public delegate void GetUsersCallback(Dictionary<string, UserBlog> users);


        /// <summary>
        /// Adds a user to the Firebase Database
        /// </summary>
        /// <param name="user"> User object that will be uploaded </param>
        /// <param name="userId"> Id of the user that will be uploaded </param>
        /// <param name="callback"> What to do after the user is uploaded successfully </param>
        public static void PostUser(UserBlog user, string userId, PostUserCallback callback, string idToken)
        {
            RestClient.Put<UserBlog>($"{databaseURL}users/{userId}.json?auth={idToken}", user).Then(response => {
                callback();
            });
        }

        /// <summary>
        /// Retrieves a user from the Firebase Database, given their id
        /// </summary>
        /// <param name="userId"> Id of the user that we are looking for </param>
        /// <param name="callback"> What to do after the user is downloaded successfully </param>
        public static void GetUser(string userId, GetUserCallback callback, string idToken)
        {
            RestClient.Get<UserBlog>($"{databaseURL}users/{userId}.json?auth={idToken}").Then(user => { callback(user); });
        }

        /// <summary>
        /// Retrieves a user from the Firebase Database, given their id
        /// </summary>
        /// <param name="userId"> Id of the user that we are looking for </param>
        /// <param name="callback"> What to do after the user is downloaded successfully </param>
        public static void GetUsers(GetUsersCallback callback)
        {
            RestClient.Get($"{databaseURL}users.json?auth={AuthHandler.idToken}").Then(response =>
            {
                var responseJson = response.Text;
                // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                // to serialize more complex types (a Dictionary, in this case)
                var data = fsJsonParser.Parse(responseJson);
                object deserialized = null;
                serializer.TryDeserialize(data, typeof(Dictionary<string, UserBlog>), ref deserialized);

                var users = deserialized as Dictionary<string, UserBlog>;
                callback(users);
            });
        }
    }
}
