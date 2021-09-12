using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Hashing.Models
{
    public interface ISaltGenerator
    {
        byte[] GenerateSalt(int saltLength);
    }
}
