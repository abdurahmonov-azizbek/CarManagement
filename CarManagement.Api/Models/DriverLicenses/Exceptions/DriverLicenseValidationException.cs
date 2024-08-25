// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class DriverLicenseValidationException : Xeption
    {
        public DriverLicenseValidationException(Xeption innerException)
            : base(message: "DriverLicense validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
