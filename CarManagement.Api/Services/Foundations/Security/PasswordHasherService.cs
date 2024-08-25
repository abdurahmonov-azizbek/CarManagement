// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using BC = BCrypt.Net.BCrypt;

namespace CarManagement.Api.Services.Foundations.Security
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password) =>
            BC.HashPassword(password);

        public bool Verify(string password, string hash) =>
            BC.Verify(password, hash);
    }
}
