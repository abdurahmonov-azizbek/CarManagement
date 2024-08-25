// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class ServiceTypeDependencyValidationException : Xeption
    {
        public ServiceTypeDependencyValidationException(Xeption innerException)
            : base(message: "ServiceType dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
