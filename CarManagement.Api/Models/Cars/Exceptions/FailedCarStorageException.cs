// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class FailedCarStorageException : Xeption
    {
        public FailedCarStorageException(Exception innerException)
            : base(message: "Failed car storage error occurred, contact support.", innerException)
        { }
    }
}