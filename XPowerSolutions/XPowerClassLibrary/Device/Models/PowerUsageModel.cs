using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XPowerClassLibrary.Device.Models
{
    public class PowerUsageModel
    {
        [DefaultValue(false)]
        public bool HasReturnInfo { get; set; }

        [DefaultValue(0.00)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public float CurrentUsage { get; set; }
    }
}
