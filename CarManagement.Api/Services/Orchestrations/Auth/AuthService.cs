// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using CarManagement.Api.Models.Users.Exceptions;
using CarManagement.Api.Services.Foundations.Authorizations;
using CarManagement.Api.Services.Foundations.Security;
using CarManagement.Api.Services.Foundations.Users;
using CarManagement.Api.Services.Orchestrations.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Services.Orchestrations.Auth
{
    public class AuthService(
        IUserService userService,
        IPasswordHasherService passwordHasherService,
        IAccessTokenGeneratorService accessTokenGeneratorService) : IAuthService
    {
        public async ValueTask<string> SignInAsync(SignInDetails signInDetails)
        {
            var user = await userService.RetrieveAllUsers()
                .FirstOrDefaultAsync(user => user.Email == signInDetails.Email);

            if (user is null)
            {
                throw new NullUserException();
            }

            if (!passwordHasherService.Verify(signInDetails.Password, user.Password))
            {
                throw new InvalidOperationException("Email or password incorrect!");
            }

            var token = accessTokenGeneratorService.GenerateToken(user);

            return token;
        }
    }
}
