using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public class MaxLengthRule : IValidationRule
    {
        private int maxLength;
        public MaxLengthRule(int maxLenght)
        {
            this.maxLength = maxLenght;
        }
        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                if (stringVal.Length > maxLength)
                {
                    string message = $"{validator.name} must be under {maxLength}.";
                    validator.AddError(message);
                    validator.AddException(new ArgumentException(message, validator.name));
                }
            }
        }
    }
}
