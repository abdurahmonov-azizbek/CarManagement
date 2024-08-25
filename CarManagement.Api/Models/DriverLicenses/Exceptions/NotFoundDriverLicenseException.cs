// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class NotFoundDriverLicenseException : Xeption
    {
        public NotFoundDriverLicenseException(Guid driverLicenseId)
            : base(message: $"Couldn't find driverLicense with id: {driverLicenseId}.")
        { }
    }
}
