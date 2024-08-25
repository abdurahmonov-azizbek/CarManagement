// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class FailedCarModelStorageException : Xeption
    {
        public FailedCarModelStorageException(Exception innerException)
            : base(message: "Failed carModel storage error occurred, contact support.", innerException)
        { }
    }
}