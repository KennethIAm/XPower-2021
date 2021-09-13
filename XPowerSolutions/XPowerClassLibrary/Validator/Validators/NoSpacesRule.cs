using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XPowerClassLibrary.Validator
{
    public class NoSpacesRule : IValidationRule
    {
        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                if (Regex.IsMatch(stringVal, @"\s"))
                {
                    string message = $"{validator.name} must not contain any spaces.";
                    validator.AddError(message);
                    validator.AddException(new ArgumentException(message, validator.name));
                }

            }
        }
    }
}
