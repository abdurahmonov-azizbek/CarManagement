// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class ServiceTypeDependencyException : Xeption
    {
        public ServiceTypeDependencyException(Exception innerException)
            : base(message: "ServiceType dependency error occured, contact support.", innerException)
        { }
    }
}