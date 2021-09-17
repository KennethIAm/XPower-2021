using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Models;
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

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceRequest request)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(new { message = "Invalid Token." });
                }

                if (request is null)
                {
                    return BadRequest(new { message = "Invalid Device Request." });
                }

                if (UsingInvalidIpAddress(request.DeviceIpAddress))
                {
                    return BadRequest(new { message = "Invalid Device Request, IPAddress not readable." });
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
                return BadRequest(new { message = "Invalid Inputs. Device was not created." });
            }
            catch (ArgumentException)
            {
                return BadRequest(new { message = "Invalid Inputs. Device was not created." });
            }
            catch (NullReferenceException nullRefEx)
            {
                return BadRequest(new { message = nullRefEx.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unknown error occurred. Device was not created." });
            }
        }
        
        [AllowAnonymous]
        [HttpPost("IAmOnline")]
        public async Task<IActionResult> DeviceOnline([FromBody] DeviceOnlineRequest onlineRequest)
        {
            try
            {
                if (onlineRequest is null)
                {
                    return BadRequest(new { message = "Invalid Device IAmOnline Request." });
                }

                if (UsingInvalidIpAddress(onlineRequest.IPAddress))
                {
                    return BadRequest(new { message = "Invalid Device IAmOnline Request, IPAddress not readable." });
                }

                IDevice device = await _deviceService.DeviceOnlineAsync(onlineRequest);

                if (device is null)
                {
                    throw new NullReferenceException("An unexpected error occurred. The device could not be handled successfully.");
                }

                return Ok(device);
            }
            catch (NullReferenceException nullRefEx)
            {
                return BadRequest(nullRefEx.Message);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unknown error occurred. Device was not set Online." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(new { message = "Invalid Token." });
                }

                if (id is 0)
                {
                    return NotFound(new { message = "No Device found." });
                }

                IDevice device = await _deviceService.GetDeviceByIdAsync(id);

                if (device is null)
                {
                    return NotFound(new { message = "No Device found." });
                }

                return Ok(device);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unknown error occurred. Device not found." });
            }
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateDevice([FromBody] UpdateDeviceRequest updateRequest)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(new { message = "Invalid Token." });
                }

                if (updateRequest is null)
                {
                    return BadRequest(new { message = "Invalid Device Update Request." });
                }

                if (UsingInvalidIpAddress(updateRequest.DeviceIpAddress))
                {
                    return BadRequest(new { message = "Invalid Device Update Request, IPAddress not readable." });
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
                return BadRequest(new { message = "Invalid Inputs. Device was not updated." });
            }
            catch (ArgumentException)
            {
                return BadRequest(new { message = "Invalid Inputs. Device was not updated." });
            }
            catch (NullReferenceException nullRefEx)
            {
                return BadRequest(new { message = nullRefEx.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unknown error occurred. Device was not updated." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                if (UsingInvalidRefreshToken())
                {
                    return Unauthorized(new { message = "Invalid Token." });
                }

                if (id is 0)
                {
                    return NotFound(new { message = "No Device found." });
                }

                bool isDeleted = await _deviceService.DeleteDeviceByIdAsync(id);

                if (!isDeleted)
                {
                    return Conflict(new { message = "Couldn't delete device." });
                }

                return Ok(new { message = "Device was successfully deleted." });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unknown error occurred. Device was not deleted" });
            }
        }

        private bool UsingInvalidRefreshToken()
        {
            var token = GetCurrentUserToken();

            return string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token);
        }

        private bool UsingInvalidIpAddress(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out IPAddress parsedAddress);
        }
    }
}
