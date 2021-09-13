using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XPowerClassLibrary.Validator;

namespace BlazorServerWebsite.Data.Models
{
    public abstract class BaseAccountModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst en gyldig e-mailadresse.")]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venglist legitimationsoplysninger.")]
        [MinLength(8, ErrorMessage = "Legitimationsoplysninger opfylder ikke vores sikkerhedskrav. Skal være minimum 8 karakterer langt.")]
        [MaxLength(64, ErrorMessage = "Legitimationsoplysninger opfylder ikke vores sikkerhedskrav. Må maksimalt være 64 karakterer langt.")]
        public string Password { get; set; }


        public virtual bool IsEmailValid()
        {
            try
            {
                DefaultValidators.ValidateMailException(EmailAddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public virtual bool IsPasswordValid()
        {
            try
            {
                DefaultValidators.ValidatePasswordException(Password);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
