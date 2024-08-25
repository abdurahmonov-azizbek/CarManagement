// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class FailedServiceServiceException : Xeption
    {
        public FailedServiceServiceException(Exception innerException)
            : base(message: "Failed service service error occurred, please contact support.", innerException)
        { }
    }
}