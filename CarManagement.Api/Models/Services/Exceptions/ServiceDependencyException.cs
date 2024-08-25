// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class ServiceDependencyException : Xeption
    {
        public ServiceDependencyException(Exception innerException)
            : base(message: "Service dependency error occured, contact support.", innerException)
        { }
    }
}