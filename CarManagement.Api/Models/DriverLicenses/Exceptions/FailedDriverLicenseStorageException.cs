// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class FailedDriverLicenseStorageException : Xeption
    {
        public FailedDriverLicenseStorageException(Exception innerException)
            : base(message: "Failed driverLicense storage error occurred, contact support.", innerException)
        { }
    }
}