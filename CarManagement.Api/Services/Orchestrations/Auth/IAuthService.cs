// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using CarManagement.Api.Services.Orchestrations.Auth.Models;

namespace CarManagement.Api.Services.Orchestrations.Auth
{
    public interface IAuthService
    {
        ValueTask<string> SignInAsync(SignInDetails signInDetails);
    }
}
