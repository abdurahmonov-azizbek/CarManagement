// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class FailedCarModelServiceException : Xeption
    {
        public FailedCarModelServiceException(Exception innerException)
            : base(message: "Failed carModel service error occurred, please contact support.", innerException)
        { }
    }
}