namespace XPowerClassLibrary.Device.Models
{
    public class AssignDeviceToUserRequest
    {
        public string UserTokenRequest { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public string DeviceName { get; set; }
        public int DeviceTypeId { get; set; }
    }
}
