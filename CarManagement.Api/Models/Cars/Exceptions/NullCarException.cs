// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class NullCarException : Xeption
    {
        public NullCarException()
            : base(message: "Car is null.")
        { }
    }
}

