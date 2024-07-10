using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.HS
{
    public class Main : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        //private static void OnAppStart()
        //{
        //    var user1 = new UserBlog("Matt", "Smith", 36);
        //    DatabaseHandler.PostUser(user1, "11", () =>
        //    {
        //        DatabaseHandler.GetUser("11", user =>
        //        {
        //            Debug.Log($"{user.name}'s age is {user.age}");
        //        });

        //        DatabaseHandler.GetUsers(users =>
        //        {
        //            foreach (var user in users)
        //            {
        //                Debug.Log($"{user.Value.name} {user.Value.surname} {user.Value.age}");
        //            }
        //        });
        //    });

        //    var user2 = new UserBlog("Peter", "Capaldi", 61);
        //    DatabaseHandler.PostUser(user2, "12", () => { });
        //}
        private static void OnAppStart()
        {
            AuthHandler.SignUp("hyung4rang@gmail.com", "1234qwer", new UserBlog("HS", "Kim", 29));
            AuthHandler.SignIn("hyung4rang@gmail.com", "1234qwer");
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}
