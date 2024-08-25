// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Services.Foundations.Security
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool Verify(string password, string hash);
    }
}
