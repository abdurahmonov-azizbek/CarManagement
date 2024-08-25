// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class CarDependencyException : Xeption
    {
        public CarDependencyException(Exception innerException)
            : base(message: "Car dependency error occured, contact support.", innerException)
        { }
    }
}