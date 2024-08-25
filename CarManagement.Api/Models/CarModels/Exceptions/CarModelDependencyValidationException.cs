// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class CarModelDependencyValidationException : Xeption
    {
        public CarModelDependencyValidationException(Xeption innerException)
            : base(message: "CarModel dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
