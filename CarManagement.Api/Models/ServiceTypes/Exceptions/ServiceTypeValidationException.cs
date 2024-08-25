// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class ServiceTypeValidationException : Xeption
    {
        public ServiceTypeValidationException(Xeption innerException)
            : base(message: "ServiceType validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
