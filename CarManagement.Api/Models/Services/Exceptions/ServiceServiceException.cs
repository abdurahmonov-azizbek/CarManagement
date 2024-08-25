// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class ServiceServiceException : Xeption
    {
        public ServiceServiceException(Exception innerException)
            : base(message: "Service service error occured, contact support.", innerException)
        { }
    }
}