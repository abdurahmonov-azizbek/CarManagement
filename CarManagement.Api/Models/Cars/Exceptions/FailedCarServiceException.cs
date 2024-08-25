// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class FailedCarServiceException : Xeption
    {
        public FailedCarServiceException(Exception innerException)
            : base(message: "Failed car service error occurred, please contact support.", innerException)
        { }
    }
}