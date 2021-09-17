using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XPowerClassLibrary.Validator;

namespace BlazorServerWebsite.Data.Models
{
    public class RegisterUnitModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst et gyldigt ID.")]
        [Range(0, int.MaxValue, ErrorMessage = "Et ID kan kun indeholde heltal.")]
        public string ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst enhedens navn.")]
        [MinLength(1, ErrorMessage = "Enhedens navn opfylder ikke vores navngivningskrav. Skal være minimum 1 karakter langt.")]
        [MaxLength(25, ErrorMessage = "Enhedens navn opfylder ikke vores navngivningskrav. Må maksimalt være 25 karakterer langt.")]
        public string Name { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst en gyldigt IP.")]
        public string IP { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst enhedens navn.")]
        [MinLength(1, ErrorMessage = "Enhedens navn opfylder ikke vores navngivningskrav. Skal være minimum 1 karakter langt.")]
        [MaxLength(25, ErrorMessage = "Enhedens navn opfylder ikke vores navngivningskrav. Må maksimalt være 25 karakterer langt.")]
        public string Type { get; set; }

        public bool IsIDValid()
        {
            try
            {
                DefaultValidators.ValidateUnitIDException(Name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsNameValid()
        {
            try
            {
                DefaultValidators.ValidateUnitNameException(Name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsTypeValid()
        {
            try
            {
                DefaultValidators.ValidateUnitTypeException(Name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsValidForm()
        {
            return IsIDValid() && IsNameValid() && IsTypeValid();
        }
    }
}
