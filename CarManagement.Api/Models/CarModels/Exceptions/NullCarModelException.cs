// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class NullCarModelException : Xeption
    {
        public NullCarModelException()
            : base(message: "CarModel is null.")
        { }
    }
}

