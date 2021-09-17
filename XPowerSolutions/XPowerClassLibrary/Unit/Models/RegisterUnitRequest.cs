using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Unit.Models
{
    public class RegisterUnitRequest
    {
        public int Id { get; set; }

        //[Required]
        public string Name { get; set; }

        //[Required]
        public string Ip { get; set; }

        //[Required]
        public string Type { get; set; }

        public RegisterUnitRequest() { }

        public RegisterUnitRequest(int id, string name, string ip, string type)
        {
            Id = id;
            Name = name;
            Ip = ip;
            Type = type;
        }
    }
}
