using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerWebsite.Data.Models
{
    public class AccountLogInModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst en gyldig email adresse.")]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venglist legitimationsoplysninger.")]
        public string Password { get; set; }
    }
}
