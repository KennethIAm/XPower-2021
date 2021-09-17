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


        static private List<IValidationRule> unitNameRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new MinLengthRule(1),
                    new MaxLengthRule(25),
                    new NoSqlInjectionRule()
                };
        static private List<IValidationRule> unitTypeRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new MinLengthRule(1),
                    new MaxLengthRule(20),
                    new NoSqlInjectionRule()
                };
        static private List<IValidationRule> unitIDRules = new List<IValidationRule>()
                {
                    new NullRule(),
                    new NoEmptyStringRule(),
                    new NoSpacesRule(),
                    new MinLengthRule(4),
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
        /// Validates the unit name.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="unitName">Unit name value to validate.</param>
        static public void ValidateUnitNameException(string unitName)
        {
            Validator validator = new Validator("UnitName", unitNameRules);
            if (!validator.Validate(unitName))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Validates the unit name.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="unitID">Unit ID value to validate.</param>
        static public void ValidateUnitIDException(string unitID)
        {
            Validator validator = new Validator("UnitID", unitIDRules);
            if (!validator.Validate(unitID))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Validates the unit name.
        /// Throws exception if an error is reached.
        /// </summary>
        /// <param name="unitType">Unit type value to validate.</param>
        static public void ValidateUnitTypeException(string unitType)
        {
            Validator validator = new Validator("UnitType", unitTypeRules);
            if (!validator.Validate(unitType))
            {
                foreach (var exception in validator.GetExceptions())
                {
                    throw exception;
                }
            }
        }
    }
}
