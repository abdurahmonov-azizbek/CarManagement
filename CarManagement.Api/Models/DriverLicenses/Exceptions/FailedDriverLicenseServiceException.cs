// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class FailedDriverLicenseServiceException : Xeption
    {
        public FailedDriverLicenseServiceException(Exception innerException)
            : base(message: "Failed driverLicense service error occurred, please contact support.", innerException)
        { }
    }
}