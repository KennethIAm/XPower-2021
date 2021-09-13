using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public class PasswordBlackListRule : IValidationRule
    {
        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                foreach (string blacklistItem in this.GetBlackList())
                {
                    if (string.Equals(stringVal.ToLower(), blacklistItem.ToLower()))
                    {
                        string message = $"{validator.name} can't be {stringVal}.";
                        validator.AddError(message);
                        validator.AddException(new ArgumentException(message, validator.name));
                    }
                }
            }
        }

        private List<string> GetBlackList()
        {
            return new List<string>()
            {
                "password",
                "12345678",
                "123456789",
                "1234567890",
                "qwerty123",
                "password123"
            };
        }
    }
}
