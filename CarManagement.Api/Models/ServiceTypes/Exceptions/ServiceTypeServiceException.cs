// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class ServiceTypeServiceException : Xeption
    {
        public ServiceTypeServiceException(Exception innerException)
            : base(message: "ServiceType service error occured, contact support.", innerException)
        { }
    }
}