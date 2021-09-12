using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public class ValidMailRule : IValidationRule
    {
        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                // This will throw an ArgumentException if the given mail is invalid.
                // This follows the email standard given by RFC#822: https://www.ietf.org/rfc/rfc0822.txt
                // Which means an email called mail@com IS valid.
                try
                {
                    MailAddress m = new MailAddress(stringVal);
                }
                catch (Exception)
                {
                    string message = $"{stringVal} is not a valid {validator.name}.";
                    validator.AddError(message);
                    validator.AddException(new ArgumentException(message, validator.name));
                }
            }
        }
    }
}
