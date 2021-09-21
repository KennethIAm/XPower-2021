using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XPowerClassLibrary.Validator;

namespace BlazorServerWebsite.Data.Models
{
    public class CreateDeviceModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst et gyldigt ID.")]
        [Range(0, int.MaxValue, ErrorMessage = "Et ID kan kun indeholde heltal.")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst enhedens navn.")]
        [MinLength(1, ErrorMessage = "Enhedens navn opfylder ikke vores navngivningskrav. Skal være minimum 1 karakter langt.")]
        [MaxLength(25, ErrorMessage = "Enhedens navn opfylder ikke vores navngivningskrav. Må maksimalt være 25 karakterer langt.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Indtast venligst enhedens navn.")]
        [MinLength(1, ErrorMessage = "Enhedens type opfylder ikke vores navngivningskrav. Skal være minimum 1 karakter langt.")]
        [MaxLength(25, ErrorMessage = "Enhedens type opfylder ikke vores navngivningskrav. Må maksimalt være 25 karakterer langt.")]
        [Range(0, int.MaxValue, ErrorMessage = "Et ID kan kun indeholde heltal.")]
        public string Type { get; set; }

        public bool IsIDValid()
        {
            try
            {
                DefaultValidators.ValidateDeviceIDException(Name);
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
                DefaultValidators.ValidateDeviceNameException(Name);
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
                DefaultValidators.ValidateDeviceTypeException(Name);
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
