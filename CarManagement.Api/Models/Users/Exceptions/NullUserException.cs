// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Users.Exceptions
{
    public class NullUserException : Xeption
    {
        public NullUserException()
            : base(message: "User is null.")
        { }
    }
}

