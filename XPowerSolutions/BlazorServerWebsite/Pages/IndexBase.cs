using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Device.Enums;

namespace BlazorServerWebsite.Pages
{
    public partial class IndexBase : ComponentBase
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [CascadingParameter] protected Task<AuthenticationState> AuthenticationState { get; set; }
        private AuthenticationState _context;

        public IEnumerable<HardwareDevice> UserDevices { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDevices();
            _context = await AuthenticationState;
        }

        protected void GoToCreateDevice()
        {
            NavigationManager.NavigateTo("/device/registerdevice");
        }

        private async Task LoadDevices()
        {
            HardwareDevice d1 = new HardwareDevice()
            { 
            Id = 1,
            DeviceType = new DeviceType(1, "Lyselement"),
            FunctionalStatus = DeviceFunctionalStatus.On,
            ConnectionState = DeviceConnectionState.Connected,
            Name = "JohnnysLampe",
            IPAddress = "Temp"
            };

            HardwareDevice d2 = new HardwareDevice()
            {
                Id = 2,
                DeviceType = new DeviceType(1, "Lyselement"),
                FunctionalStatus = DeviceFunctionalStatus.On,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Badeværelseslys",
                IPAddress = "Temp"
            };

            HardwareDevice d3 = new HardwareDevice()
            {
                Id = 3,
                DeviceType = new DeviceType(1, "Lyselement"),
                FunctionalStatus = DeviceFunctionalStatus.On,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Soveværelseslampe",
                IPAddress = "Temp"
            };

            HardwareDevice d4 = new HardwareDevice()
            {
                Id = 4,
                DeviceType = new DeviceType(1, "Lyselement"),
                FunctionalStatus = DeviceFunctionalStatus.Off,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Køkkenlys",
                IPAddress = "Temp"
            };

            HardwareDevice d5 = new HardwareDevice()
            {
                Id = 5,
                DeviceType = new DeviceType(4, "Klimaelement"),
                FunctionalStatus = DeviceFunctionalStatus.On,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Blæser",
                IPAddress = "Temp"
            };

            UserDevices = new List<HardwareDevice> { d1, d2, d3, d4, d5 };
        }
    }
}
