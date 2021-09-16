using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XPowerClassLibrary.Validator;

namespace BlazorServerWebsite.Data.Models
{
    public class AccountRegisterModel : BaseAccountModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst et brugernavn.")]
        [MinLength(1, ErrorMessage = "Brugernavn opfylder ikke vores navngivnings krav. Skal være minimum 1 karakterer langt.")]
        [MaxLength(30, ErrorMessage = "Brugernavn opfylder ikke vores navngivnings krav. Må maksimalt være 30 karakterer langt.")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage = "Venligst genindtast legitimationsoplysninger.")]
        [Compare(nameof(Password), ErrorMessage = "De indtastede legitimationsoplysninger stemmer ikke overens.")]
        [PasswordPropertyText]
        public string ReEnterPassword { get; set; }

        public bool IsUsernameValid()
        {
            try
            {
                DefaultValidators.ValidateUsernameException(Username);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool IsValidForm()
        {
            return IsUsernameValid() && IsEmailValid() && IsPasswordValid();
        }
    }
}
