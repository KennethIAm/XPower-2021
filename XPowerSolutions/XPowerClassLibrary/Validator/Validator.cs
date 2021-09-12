using System;
using System.Collections.Generic;
using System.Text;
using XPowerClassLibrary.Validator;

namespace XPowerClassLibrary.Validator
{
    /// <summary>
    /// Class to handle validations using IValidationRules given
    /// </summary>
    public class Validator
    {
        internal string name { get; private set; }

        private List<string> listOfErrors = new List<string>();
        private List<Exception> listOfExceptions = new List<Exception>();
        private IValidationRule[] rules;

        /// <summary>
        /// Class to handle validations using IValidationRules given
        /// </summary>
        /// <param name="name">the name of the value to validate</param>
        /// <param name="rules">the rules that will be used to validate</param>
        public Validator(string name, List<IValidationRule> rules)
        {
            this.name = name;
            this.rules = rules.ToArray();
        }

        /// <summary>
        /// validates the given obejct
        /// </summary>
        /// <param name="value">the object to validate</param>
        /// <returns></returns>
        public bool Validate(object value)
        {
            // Clears the lists before validating
            listOfErrors.Clear();
            listOfExceptions.Clear();

            bool validationWasSuccessfull = false;
            
            // This has to be set to a varible in order to be used as ref
            Validator validator = this;

            // Checks at each rule
            foreach (IValidationRule rule in this.rules)
            {
                // The rule will add exception and error to the lists if the rule is broken
                rule.Check(value, ref validator);
            }

            if (this.listOfErrors.Count == 0 && this.listOfExceptions.Count == 0)
            {
                validationWasSuccessfull = true;
            }

            return validationWasSuccessfull;
        }

        /// <summary>
        /// gets the error list for last validation
        /// </summary>
        /// <returns>The list errors as string list</returns>
        public List<string> GetErrors()
        {
            return this.listOfErrors;
        }


        /// <summary>
        /// gets the exception list for last validation
        /// </summary>
        /// <returns>The list Execptions</returns>
        public List<Exception> GetExceptions()
        {
            return this.listOfExceptions;
        }

        internal void AddException(Exception exception)
        {
            this.listOfExceptions.Add(exception);
        }

        internal void AddError(string error)
        {
            this.listOfErrors.Add(error);
        }
    }
}
