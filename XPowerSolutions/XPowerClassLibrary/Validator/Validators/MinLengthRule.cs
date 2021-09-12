using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public class MinLengthRule : IValidationRule
    {
        private int minLength;
        public MinLengthRule(int minLenght)
        {
            this.minLength = minLenght;
        }
        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                if (stringVal.Length < minLength)
                {
                    string message = $"{validator.name} must be atleast {minLength}.";
                    validator.AddError(message);
                    validator.AddException(new ArgumentException(message, validator.name));
                }
            }
        }
    }
}
