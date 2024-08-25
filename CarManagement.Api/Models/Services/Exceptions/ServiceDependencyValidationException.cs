// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class ServiceDependencyValidationException : Xeption
    {
        public ServiceDependencyValidationException(Xeption innerException)
            : base(message: "Service dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
