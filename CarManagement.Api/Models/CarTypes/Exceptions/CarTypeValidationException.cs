// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class CarTypeValidationException : Xeption
    {
        public CarTypeValidationException(Xeption innerException)
            : base(message: "CarType validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
