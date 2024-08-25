// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class CarTypeServiceException : Xeption
    {
        public CarTypeServiceException(Exception innerException)
            : base(message: "CarType service error occured, contact support.", innerException)
        { }
    }
}