// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class NullCarTypeException : Xeption
    {
        public NullCarTypeException()
            : base(message: "CarType is null.")
        { }
    }
}

