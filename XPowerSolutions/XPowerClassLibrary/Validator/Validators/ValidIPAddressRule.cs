using System;
using System.Net;

namespace XPowerClassLibrary.Validator
{
    public class ValidIPAddressRule : IValidationRule
    {
        private readonly int _maxStringLength = 15;
        private string _ipAddress = string.Empty;

        public void Check(object value, ref Validator validator)
        {
            if (value is string stringVal)
            {
                if (InvalidIPAddress(stringVal))
                {
                    string message = $"{validator.name} is not a valid ip address. Got {_ipAddress}";
                    validator.AddError(message);
                    validator.AddException(new ArgumentException(message, validator.name));

                    return;
                }
            }
        }

        private bool InvalidIPAddress(string value)
        {
            var isInvalid = !IPAddress.TryParse(value, out IPAddress address);
            _ipAddress = address.ToString();

            return isInvalid;
        }
    }
}
