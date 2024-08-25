// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class NullDriverLicenseException : Xeption
    {
        public NullDriverLicenseException()
            : base(message: "DriverLicense is null.")
        { }
    }
}

