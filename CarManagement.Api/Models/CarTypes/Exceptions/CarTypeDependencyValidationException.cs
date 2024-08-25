// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class CarTypeDependencyValidationException : Xeption
    {
        public CarTypeDependencyValidationException(Xeption innerException)
            : base(message: "CarType dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
