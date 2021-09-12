using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public class NoEmptyStringRule : IValidationRule
    {
        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                if (string.IsNullOrEmpty(stringVal))
                {
                    string message = $"{validator.name} must not be empty.";
                    validator.AddError(message);
                    validator.AddException(new ArgumentException(message, validator.name));
                }
            }
        }
    }
}
