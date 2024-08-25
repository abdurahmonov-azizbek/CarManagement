// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class CarModelValidationException : Xeption
    {
        public CarModelValidationException(Xeption innerException)
            : base(message: "CarModel validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
