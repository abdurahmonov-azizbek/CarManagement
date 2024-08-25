// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class InvalidDriverLicenseException : Xeption
    {
        public InvalidDriverLicenseException()
            : base(message: "DriverLicense is invalid.")
        { }
    }
}
