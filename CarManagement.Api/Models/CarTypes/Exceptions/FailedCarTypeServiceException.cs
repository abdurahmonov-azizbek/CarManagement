// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class FailedCarTypeServiceException : Xeption
    {
        public FailedCarTypeServiceException(Exception innerException)
            : base(message: "Failed carType service error occurred, please contact support.", innerException)
        { }
    }
}