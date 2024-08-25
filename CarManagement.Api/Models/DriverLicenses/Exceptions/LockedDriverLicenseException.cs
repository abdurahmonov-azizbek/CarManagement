// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class LockedDriverLicenseException : Xeption
    {
        public LockedDriverLicenseException(Exception innerException)
            : base(message: "DriverLicense is locked, please try again.", innerException)
        { }
    }
}
