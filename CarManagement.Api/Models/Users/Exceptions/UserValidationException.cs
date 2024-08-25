// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Users.Exceptions
{
    public class UserValidationException : Xeption
    {
        public UserValidationException(Xeption innerException)
            : base(message: "User validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
