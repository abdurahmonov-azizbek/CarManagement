// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class ServiceValidationException : Xeption
    {
        public ServiceValidationException(Xeption innerException)
            : base(message: "Service validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
