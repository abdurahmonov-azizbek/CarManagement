// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Users.Exceptions
{
    public class InvalidUserException : Xeption
    {
        public InvalidUserException()
            : base(message: "User is invalid.")
        { }
    }
}
