using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public class NullRule : IValidationRule
    {
        public void Check(object value, ref Validator validator)
        {
            if(value == null)
            {
                string message = $"{validator.name} must not be null.";
                validator.AddError(message);
                validator.AddException(new ArgumentNullException(validator.name, message));
            }
        }
    }
}
