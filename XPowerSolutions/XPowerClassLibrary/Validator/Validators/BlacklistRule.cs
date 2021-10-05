using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    class BlacklistRule : IValidationRule
    {
        private IEnumerable<string> _bannedValueList;

        public BlacklistRule(IEnumerable<string> bannedValueList)
        {
            this._bannedValueList = bannedValueList;
        }

        public void Check(object value, ref Validator validator)
        {
            // IF Check if value exists in banned list
            if (value == null)
                return;

            if (this._bannedValueList.Any(x => x.ToUpper().Contains( value.ToString().ToUpper() )))
            {
                string errorMessage = $"The mail: {value}, is blacklisted.";

                // Add ArgumentException to exception list
                validator.AddException(new ArgumentException(errorMessage, nameof(value)));

                // Add message to error list 
                validator.AddError(errorMessage);
            }

            // Else do nothing 

        }
    }
}
