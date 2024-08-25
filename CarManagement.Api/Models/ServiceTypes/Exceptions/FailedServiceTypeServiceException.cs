// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class FailedServiceTypeServiceException : Xeption
    {
        public FailedServiceTypeServiceException(Exception innerException)
            : base(message: "Failed serviceType service error occurred, please contact support.", innerException)
        { }
    }
}