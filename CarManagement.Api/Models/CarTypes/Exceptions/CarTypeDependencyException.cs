// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class CarTypeDependencyException : Xeption
    {
        public CarTypeDependencyException(Exception innerException)
            : base(message: "CarType dependency error occured, contact support.", innerException)
        { }
    }
}