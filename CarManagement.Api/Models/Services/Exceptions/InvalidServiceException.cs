// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class InvalidServiceException : Xeption
    {
        public InvalidServiceException()
            : base(message: "Service is invalid.")
        { }
    }
}
