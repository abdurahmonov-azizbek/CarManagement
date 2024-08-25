// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class DriverLicenseDependencyValidationException : Xeption
    {
        public DriverLicenseDependencyValidationException(Xeption innerException)
            : base(message: "DriverLicense dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
