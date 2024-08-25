// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class FailedCarTypeStorageException : Xeption
    {
        public FailedCarTypeStorageException(Exception innerException)
            : base(message: "Failed carType storage error occurred, contact support.", innerException)
        { }
    }
}