// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using CarManagement.Api.Models.Users.Exceptions;
using CarManagement.Api.Services.Orchestrations.Auth;
using CarManagement.Api.Services.Orchestrations.Auth.Models;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService) : RESTFulController
    {
        [HttpGet("sign-in")]
        public async ValueTask<IActionResult> SignIn([FromQuery] SignInDetails signInDetails)
        {
            try
            {
                var result = await authService.SignInAsync(signInDetails);

                return Ok(result);
            }
            catch (NullUserException nullUserException)
            {
                return NotFound(nullUserException);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return BadRequest(invalidOperationException);
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }
    }
}
