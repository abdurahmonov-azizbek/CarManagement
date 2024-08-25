// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class CarServiceException : Xeption
    {
        public CarServiceException(Exception innerException)
            : base(message: "Car service error occured, contact support.", innerException)
        { }
    }
}