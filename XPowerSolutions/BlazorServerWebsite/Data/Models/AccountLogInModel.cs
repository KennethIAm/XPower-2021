using System.ComponentModel.DataAnnotations;

namespace BlazorServerWebsite.Data.Models
{
    public class AccountLogInModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst en gyldig e-mailadresse.")]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venglist legitimationsoplysninger.")]
        public string Password { get; set; }
    }
}
