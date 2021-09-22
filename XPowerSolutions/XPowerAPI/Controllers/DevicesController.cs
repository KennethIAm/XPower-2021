using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Models.Requests;
using XPowerClassLibrary.Device.Services;
using XPowerClassLibrary.Validator;

namespace XPowerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly IHttpClientFactory _clientFactory;

        public DevicesController(IDeviceService deviceService, IHttpClientFactory clientFactory)
        {
            _deviceService = deviceService;
            _clientFactory = clientFactory;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceRequest request)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(GenerateExceptionMessage("Invalid Token."));
                }

                if (request is null)
                {
                    return BadRequest(GenerateExceptionMessage("Invalid Device Request."));
                }

                if (UsingValidIpAddress(request.DeviceIpAddress))
                {
                    return BadRequest(GenerateExceptionMessage("Invalid Device Request, IPAddress not readable."));
                }

                IDevice device = await _deviceService.CreateDeviceAsync(request);

                if (device is null)
                {
                    throw new NullReferenceException("An unexpected error occurred. The device creation could not be handled successfully.");
                }

                return Ok(device);
            }
            catch (ArgumentNullException)
            {
                return BadRequest(GenerateExceptionMessage("Invalid Inputs. Device was not created."));
            }
            catch (ArgumentException)
            {
                return BadRequest(GenerateExceptionMessage("Invalid Inputs. Device was not created."));
            }
            catch (NullReferenceException nullRefEx)
            {
                return BadRequest(GenerateExceptionMessage(nullRefEx.Message));
            }
            catch (Exception)
            {
                return BadRequest(GenerateExceptionMessage("Unknown error occurred. Device was not created."));
            }
        }
        
        [AllowAnonymous]
        [HttpGet("IAmOnline")]
        public async Task<IActionResult> DeviceOnline([FromQuery] DeviceOnlineRequest onlineRequest)
        {
            try
            {
                if (onlineRequest is null)
                {
                    return BadRequest(GenerateExceptionMessage("Invalid Device IAmOnline Request."));
                }

                if (!UsingValidIpAddress(onlineRequest.IPAddress))
                {
                    return BadRequest(GenerateExceptionMessage("Invalid Device IAmOnline Request, IPAddress not readable."));
                }

                IDevice device = await _deviceService.DeviceOnlineAsync(onlineRequest);

                if (device is null)
                {
                    throw new NullReferenceException("An unexpected error occurred. The device could not be handled successfully.");
                }

                Console.WriteLine($"{onlineRequest.IPAddress} is now Online {onlineRequest.UniqueDeviceIdentifier}");
                return Ok(device);
            }
            catch (NullReferenceException nullRefEx)
            {
                return BadRequest(GenerateExceptionMessage(nullRefEx.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(GenerateExceptionMessage(ex.Message));
            }
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetUsersDevices([FromBody] UserDevicesRequest devicesRequest)
        {
            try
            {
                if (devicesRequest is null)
                    return BadRequest(GenerateExceptionMessage("Invalid data."));

                if (string.IsNullOrEmpty(devicesRequest.RefreshToken))
                    return BadRequest(GenerateExceptionMessage("Couldn't authorize request. Invalid token."));

                var usersDevices = await _deviceService.GetUsersOwnedDevices(devicesRequest);

                if (usersDevices is null)
                    return NotFound(GenerateExceptionMessage("Couldn't find any owned devices."));

                return Ok(usersDevices);
            }
            catch (NullReferenceException)
            {
                return BadRequest(GenerateExceptionMessage("An error ocurred while collection the user."));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(GenerateExceptionMessage("An unhandled exception occurred, couldn't successfully complete request."));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(GenerateExceptionMessage("Invalid Token."));
                }

                if (id is 0)
                {
                    return NotFound(GenerateExceptionMessage("No Device found."));
                }

                IDevice device = await _deviceService.GetDeviceByIdAsync(id);

                if (device is null)
                {
                    return NotFound(GenerateExceptionMessage("No Device found."));
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                return BadRequest(GenerateExceptionMessage(ex.Message));
            }
        }

        [HttpGet("send-command")]
        public async Task<IActionResult> SendDeviceCommand([FromQuery] string command, string ipAddress)
        {
            try
            {
                var uri = new Uri($"http://{ipAddress}");
                var response = await _clientFactory.CreateClient().GetAsync($"{uri}?{command}", HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }

                return NotFound(GenerateExceptionMessage("Command not found."));
            }
            catch (Exception ex)
            {

                return BadRequest(GenerateExceptionMessage($"Couldn't execute device command: {ex.Message}"));
            }
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateDevice([FromBody] UpdateDeviceRequest updateRequest)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(GenerateExceptionMessage("Invalid Token."));
                }

                if (updateRequest is null)
                {
                    return BadRequest(GenerateExceptionMessage("Invalid Device Update Request."));
                }

                if (UsingValidIpAddress(updateRequest.DeviceIpAddress))
                {
                    return BadRequest(GenerateExceptionMessage("Invalid Device Update Request, IPAddress not readable."));
                }

                IDevice device = await _deviceService.UpdateDeviceAsync(updateRequest);

                if (device is null)
                {
                    throw new NullReferenceException("An unexpected error occurred. The device update could not be handled successfully.");
                }

                return Ok(device);
            }
            catch (ArgumentNullException)
            {
                return BadRequest(GenerateExceptionMessage("Invalid Inputs. Device was not updated."));
            }
            catch (ArgumentException)
            {
                return BadRequest(GenerateExceptionMessage("Invalid Inputs. Device was not updated."));
            }
            catch (NullReferenceException nullRefEx)
            {
                return BadRequest(GenerateExceptionMessage(nullRefEx.Message));
            }
            catch (Exception)
            {
                return BadRequest(GenerateExceptionMessage("Unknown error occurred. Device was not updated."));
            }
        }

        [HttpPut("assign-to-me")]
        public async Task<IActionResult> AssignDeviceToUser([FromBody] AssignDeviceToUserRequest assignDeviceRequest)
        {
            try
            {
                if (assignDeviceRequest is null)
                    return BadRequest(GenerateExceptionMessage("Invalid Data Given."));

                if (string.IsNullOrEmpty(assignDeviceRequest.UserTokenRequest) || string.IsNullOrEmpty(assignDeviceRequest.UniqueDeviceIdentifier))
                    return NotFound(GenerateExceptionMessage("Data couldn't be found."));

                IDevice assignedDevice = await _deviceService.AssignDeviceToUserAsync(assignDeviceRequest);

                if (assignedDevice is null)
                    return BadRequest(GenerateExceptionMessage("Something went wrong while assigning device to user."));

                return Ok(assignedDevice);
            }
            catch (DuplicateNameException ex)
            {
                return Conflict(GenerateExceptionMessage(ex.Message));
            }
            catch (Exception ex)
            {
                //return BadRequest("An error occurred. Couldn't handle the request.");
                return BadRequest(GenerateExceptionMessage(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(GenerateExceptionMessage("Invalid Token."));
                }

                if (id is 0)
                {
                    return NotFound(GenerateExceptionMessage("No Device found."));
                }

                bool isDeleted = await _deviceService.DeleteDeviceByIdAsync(id);

                if (!isDeleted)
                {
                    return Conflict(GenerateExceptionMessage("Couldn't delete device."));
                }

                return Ok(GenerateExceptionMessage("Device was successfully deleted."));
            }
            catch (Exception)
            {
                return BadRequest(GenerateExceptionMessage("Unknown error occurred. Device was not deleted"));
            }
        }

        private bool UsingInvalidRefreshToken()
        {
            var token = GetCurrentUserToken();

            return string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token);
        }

        private bool UsingValidIpAddress(string ipAddress)
        {
            try
            {
                var address = IPAddress.Parse(ipAddress);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
