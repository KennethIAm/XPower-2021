using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Validator
{
    public interface IValidationRule
    {
        void Check(object value, ref Validator validator);
    }
}
