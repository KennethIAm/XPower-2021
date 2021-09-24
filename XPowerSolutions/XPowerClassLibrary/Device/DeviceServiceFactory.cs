using System.Data.SqlClient;
using XPowerClassLibrary.Device.Repository;
using XPowerClassLibrary.Device.Services;
using XPowerClassLibrary.Users;

namespace XPowerClassLibrary.Device
{
    public static class DeviceServiceFactory
    {
        /// <summary>
        /// SqlConnection with permission to create new device via SPCreateNewDevice
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionCreateDevice()
        {
            string username = "DeviceCreator";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permission to basic select.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionBasicReader()
        {
            string username = "DeviceBasicReader";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permission to complex select, including View and select SP.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionComplexSelect()
        {
            string username = "DeviceComplexReader";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permissions to update devices.
        /// </summary>
        /// <returns></returns>
        internal static SqlConnection GetSqlConnectionUpdateDevice()
        {
            string username = "DeviceUpdate";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permission to delete devices.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionDeleteDevice()
        {
            string username = "DeviceDelete";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        public static IDeviceService GetDeviceServiceDB()
        {
            return new DeviceService(new DbDeviceRepository(), UserServiceFactory.GetUserServiceDB());
        }
    }
}
