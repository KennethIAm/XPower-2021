using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    /// <summary>
    /// Contains business logic with set rules for validating fields.
    /// </summary>
    public class DefaultValidators
    {
        static private List<IValidationRule> mailRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new MaxLengthRule(250),
                    new ValidMailRule(),
                    new NoSqlInjectionRule()
                };
        static private List<IValidationRule> passwordRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new MinLengthRule(8),
                    new MaxLengthRule(64),
                    new PasswordBlackListRule()
                };
        static private List<IValidationRule> usernameRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new NoSpacesRule(),
                    new MinLengthRule(4),
                    new MaxLengthRule(250),
                    new NoSqlInjectionRule()
                };
        static private List<IValidationRule> refreshTokenRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new NoSpacesRule(),
                    new MinLengthRule(50),
                    new MaxLengthRule(500)
                };
        private static List<IValidationRule> _iPAddressRules = new List<IValidationRule>
        {
            new NullRule(),
            new NoEmptyStringRule(),
            new NoSpacesRule(),
            new MaxLengthRule(15),
            new MinLengthRule(7),
            new ValidIPAddressRule()
        };


        static private List<IValidationRule> deviceNameRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new NoSpacesRule(),
                    new MinLengthRule(5),
                    new MaxLengthRule(25),
                    new NoSqlInjectionRule()
                };
        static private List<IValidationRule> deviceTypeRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new MinLengthRule(1),
                    new MaxLengthRule(20),
                    new NoSqlInjectionRule()
                };
        static private List<IValidationRule> deviceIDRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new NoSpacesRule(),
                    new MinLengthRule(1),
                    new MaxLengthRule(250),
                    new NoSqlInjectionRule()
                };

        /// <summary>
        /// Validates the mail.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="mail">Mail value to validate.</param>
        static public void ValidateMailException(string mail)
        {
            Validator validator = new Validator("Mail", mailRules);
            if (!validator.Validate(mail))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Validates the password.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="password">password value to validate.</param>
        static public void ValidatePasswordException(string password)
        {
            Validator validator = new Validator("Password", passwordRules);
            if (!validator.Validate(password))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Validates the username.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="username">username value to validate.</param>
        static public void ValidateUsernameException(string username)
        {
            Validator validator = new Validator("Username", usernameRules);
            if (!validator.Validate(username))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Validates the refresh token.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="token">username value to validate.</param>
        static public void ValidateRefreshTokenException(string token)
        {
            Validator validator = new Validator("Refresh Token", refreshTokenRules);
            if (!validator.Validate(token))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        public static void ValidateIPaddressException(string iPAddress)
        {
            Validator validator = new Validator("IPAddress", _iPAddressRules);

            if (!validator.Validate(iPAddress))
            {
                foreach (var ex in validator.GetExceptions())
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Validates the mail.
        /// </summary>
        /// <param name="mail">Mail value to validate.</param>
        /// <returns>A list of all the errors</returns>
        static public List<string> ValidateMail(string mail)
        {
            Validator validator = new Validator("Mail", mailRules);
            validator.Validate(mail);
            return validator.GetErrors();
        }

        public static List<string> ValidIPAddress(string iPAddress)
        {
            Validator validator = new Validator("IPAddress", _iPAddressRules);
            validator.Validate(iPAddress);
            return validator.GetErrors();
        }

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <param name="password">password value to validate.</param>
        /// <returns>A list of all the errors</returns>
        static public List<string> ValidatePassword(string password)
        {
            Validator validator = new Validator("Password", passwordRules);
            validator.Validate(password);
            return validator.GetErrors();
        }

        /// <summary>
        /// Validates the username.
        /// </summary>
        /// <param name="username">username value to validate.</param>
        /// <returns>A list of all the errors</returns>
        static public List<string> ValidateUsername(string username)
        {
            Validator validator = new Validator("Username", usernameRules);
            validator.Validate(username);
            return validator.GetErrors();
        }

        /// <summary>
        /// Validates the device name.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="deviceName">Device name value to validate.</param>
        static public void ValidateDeviceNameException(string deviceName)
        {
            Validator validator = new Validator("DeviceName", deviceNameRules);
            if (!validator.Validate(deviceName))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Validates the device ID.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="deviceID">Device ID value to validate.</param>
        static public void ValidateDeviceIDException(string deviceID)
        {
            Validator validator = new Validator("DeviceID", deviceIDRules);
            if (!validator.Validate(deviceID))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Validates the device type.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="deviceType">Device type value to validate.</param>
        static public void ValidateDeviceTypeException(string deviceType)
        {
            Validator validator = new Validator("DeviceType", deviceTypeRules);
            if (!validator.Validate(deviceType))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }
    }
}
