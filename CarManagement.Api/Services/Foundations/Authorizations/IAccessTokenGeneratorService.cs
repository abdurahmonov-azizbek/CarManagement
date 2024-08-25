// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using CarManagement.Api.Models.Users;

namespace CarManagement.Api.Services.Foundations.Authorizations
{
    public interface IAccessTokenGeneratorService
    {
        string GenerateToken(User user);
    }
}
