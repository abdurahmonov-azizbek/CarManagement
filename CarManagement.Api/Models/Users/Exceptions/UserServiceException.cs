// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Users.Exceptions
{
    public class UserServiceException : Xeption
    {
        public UserServiceException(Exception innerException)
            : base(message: "User service error occured, contact support.", innerException)
        { }
    }
}