// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class CarModelServiceException : Xeption
    {
        public CarModelServiceException(Exception innerException)
            : base(message: "CarModel service error occured, contact support.", innerException)
        { }
    }
}