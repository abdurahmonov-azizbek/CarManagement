// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class InvalidCarModelException : Xeption
    {
        public InvalidCarModelException()
            : base(message: "CarModel is invalid.")
        { }
    }
}
