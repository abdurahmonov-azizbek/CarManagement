// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class FailedServiceTypeStorageException : Xeption
    {
        public FailedServiceTypeStorageException(Exception innerException)
            : base(message: "Failed serviceType storage error occurred, contact support.", innerException)
        { }
    }
}