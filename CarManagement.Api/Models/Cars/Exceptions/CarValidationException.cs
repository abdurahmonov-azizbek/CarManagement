// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class CarValidationException : Xeption
    {
        public CarValidationException(Xeption innerException)
            : base(message: "Car validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
