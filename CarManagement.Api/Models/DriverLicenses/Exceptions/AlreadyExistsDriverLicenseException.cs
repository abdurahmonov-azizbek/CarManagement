// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class AlreadyExistsDriverLicenseException : Xeption
    {
        public AlreadyExistsDriverLicenseException(Exception innerException)
            : base(message: "DriverLicense already exists.", innerException)
        { }
    }
}
