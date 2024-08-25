// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Services.Orchestrations.Auth.Models
{
    public class SignInDetails
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
