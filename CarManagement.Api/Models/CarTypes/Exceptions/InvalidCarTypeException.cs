// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class InvalidCarTypeException : Xeption
    {
        public InvalidCarTypeException()
            : base(message: "CarType is invalid.")
        { }
    }
}
