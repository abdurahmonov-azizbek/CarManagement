// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class CarDependencyValidationException : Xeption
    {
        public CarDependencyValidationException(Xeption innerException)
            : base(message: "Car dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
