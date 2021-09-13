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
                    new MaxLengthRule(500),
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
                    new MaxLengthRule(500),
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

    }
}
