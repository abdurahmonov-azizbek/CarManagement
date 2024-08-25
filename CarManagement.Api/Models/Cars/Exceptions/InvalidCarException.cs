// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class InvalidCarException : Xeption
    {
        public InvalidCarException()
            : base(message: "Car is invalid.")
        { }
    }
}
