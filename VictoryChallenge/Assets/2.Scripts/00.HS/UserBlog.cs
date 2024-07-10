using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VictoryChallenge.Scripts.HS
{
    public class UserBlog
    { 
        public string name;
        public string surname;
        public int age;

        public UserBlog(string name, string surname, int age)
        {
            this.name = name;
            this.surname = surname;
            this.age = age;
        }
    }
}
