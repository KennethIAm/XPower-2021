using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public class NoSqlInjectionRule : IValidationRule
    {
        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                if (this.ContainsIllegalCharacters(stringVal))
                {
                    string message = $"{validator.name} contains illegal characters.";
                    validator.AddError(message);
                    validator.AddException(new ArgumentException(message, validator.name));
                }
            }
        }

        private bool ContainsIllegalCharacters(string data)
        {
            // Check to see whether the data contains illegal character
            // or the string for making comment such as "--" or "/*"
            char[] illegalChars = { ';', '\'', '\\', '"', '=', '%', '_', '*', '\'',};
            if ((data.IndexOfAny(illegalChars) != -1) || data.Contains("--") || data.Contains("/*"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
