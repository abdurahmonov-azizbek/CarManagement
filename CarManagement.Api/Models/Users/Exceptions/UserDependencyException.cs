// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Users.Exceptions
{
    public class UserDependencyException : Xeption
    {
        public UserDependencyException(Exception innerException)
            : base(message: "User dependency error occured, contact support.", innerException)
        { }
    }
}