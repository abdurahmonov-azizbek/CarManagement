// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class DriverLicenseServiceException : Xeption
    {
        public DriverLicenseServiceException(Exception innerException)
            : base(message: "DriverLicense service error occured, contact support.", innerException)
        { }
    }
}