// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.DriverLicenses.Exceptions
{
    public class DriverLicenseDependencyException : Xeption
    {
        public DriverLicenseDependencyException(Exception innerException)
            : base(message: "DriverLicense dependency error occured, contact support.", innerException)
        { }
    }
}