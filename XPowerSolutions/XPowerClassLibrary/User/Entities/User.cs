using XPowerClassLibrary.Users.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Users.Entities
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // An empty constructor must be used, in order to use Database entitiy mapper.
        public User() { }

        public User(string mail, string username, string password) 
        {
            this.Mail = mail;
            this.Username = username;
            this.Password = password;
        }
    }
}
