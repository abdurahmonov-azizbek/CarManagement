// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Users.Exceptions
{
    public class UserDependencyValidationException : Xeption
    {
        public UserDependencyValidationException(Xeption innerException)
            : base(message: "User dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
