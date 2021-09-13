using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XPowerClassLibrary.Users.Entities
{
    [Table("AuthUsersView")]
    class AuthUsersView
    {
        [Key]
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
