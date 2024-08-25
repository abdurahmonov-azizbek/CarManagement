// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Users.Exceptions
{
    public class LockedUserException : Xeption
    {
        public LockedUserException(Exception innerException)
            : base(message: "User is locked, please try again.", innerException)
        { }
    }
}
