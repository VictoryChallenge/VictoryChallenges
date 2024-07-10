using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace VictoryChallenge.Scripts.HS
{
    public class AuthHandler
    {
        private const string _apiKey = "AIzaSyDOW8GeHJtRjVRRZAU25vPG2YRee6NcVXg";
        
        private static fsSerializer serializer = new fsSerializer();

        public delegate void EmailVerificationSuccess();
        public delegate void EmailVerificationFail();

        public static string idToken; // Key that proves the request is authenticated and the identity of the user
        public static string userId;


        public static void SignUp(string email, string password, UserBlog user)
        {
            var payLoad = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
            RestClient.Post($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key={_apiKey}",
                payLoad).Then(
                response =>
                {
                    Debug.Log("Created User");

                    var responseJson = response.Text;
                    // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                    // to serialize more complex types (a Dictionary, in this case)
                    var data = fsJsonParser.Parse(responseJson);
                    object deserialized = null;
                    serializer.TryDeserialize(data, typeof(Dictionary<string, string>), ref deserialized);

                    var authResponse = deserialized as Dictionary<string, string>;

                    DatabaseHandler.PostUser(user, authResponse["localId"], () => { }, authResponse["idToken"]);

                    SendEmailVerification(authResponse["idToken"]);
                });
        }

        /// <summary>
        /// Sends verification email
        /// </summary>
        /// <param name="newIdToken"> User's token, retrieved from SignUp </param>
        private static void SendEmailVerification(string newIdToken)
        {
            var payLoad = $"{{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"{newIdToken}\"}}";
            RestClient.Post(
                $"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key={_apiKey}", payLoad);
        }

        /// <summary>
        /// Signs in the user with Firebase Auth using Email and Password method
        /// Checks if user accepted verification email
        /// If that's the case, logs the user's object (name, surname, age)  
        /// </summary>
        /// <param name="email"> User's email </param>
        /// <param name="password"> User's password </param>
        public static void SignIn(string email, string password)
        {
            var payLoad = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
            RestClient.Post($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key={_apiKey}",
                payLoad).Then(
                response =>
                {
                    var responseJson = response.Text;
                    // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                    // to serialize more complex types (a Dictionary, in this case)
                    var data = fsJsonParser.Parse(responseJson);
                    object deserialized = null;
                    serializer.TryDeserialize(data, typeof(Dictionary<string, string>), ref deserialized);
                    var authResponse = deserialized as Dictionary<string, string>;
                    CheckEmailVerification(authResponse["idToken"], () =>
                    {
                        Debug.Log("Email verified, getting user info");
                        DatabaseHandler.GetUser(userId, user => { Debug.Log($"{user.name}, {user.surname}, {user.age}"); }, idToken);
                    }, () => { Debug.Log("Email not verified"); });
                });
        }

        /// <summary>
        /// Checks if user accepted verification email
        /// </summary>
        /// <param name="newIdToken"> User's token, retrieved from SignIn </param>
        /// <param name="callback"> What to do after acknowledging the user has verified the email </param>
        /// <param name="fallback"> What to do after acknowledging the user hasn't verified the email </param>
        private static void CheckEmailVerification(string newIdToken, EmailVerificationSuccess callback,
            EmailVerificationFail fallback)
        {
            var payLoad = $"{{\"idToken\":\"{newIdToken}\"}}";
            RestClient.Post($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key={_apiKey}",
                payLoad).Then(
                response =>
                {
                    var responseJson = response.Text;
                    // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                    // to serialize more complex types (UserData, in this case)
                    var data = fsJsonParser.Parse(responseJson);
                    object deserialized = null;
                    serializer.TryDeserialize(data, typeof(UserData), ref deserialized);
                    var authResponse = deserialized as UserData;
                    if (authResponse.users[0].emailVerified)
                    {
                        userId = authResponse.users[0].localId;
                        idToken = newIdToken;
                        callback();
                    }
                    else
                    {
                        fallback();
                    }
                });
        }
    }

}
