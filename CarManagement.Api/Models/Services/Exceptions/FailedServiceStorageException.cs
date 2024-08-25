// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class FailedServiceStorageException : Xeption
    {
        public FailedServiceStorageException(Exception innerException)
            : base(message: "Failed service storage error occurred, contact support.", innerException)
        { }
    }
}