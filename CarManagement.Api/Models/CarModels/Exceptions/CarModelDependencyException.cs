// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class CarModelDependencyException : Xeption
    {
        public CarModelDependencyException(Exception innerException)
            : base(message: "CarModel dependency error occured, contact support.", innerException)
        { }
    }
}