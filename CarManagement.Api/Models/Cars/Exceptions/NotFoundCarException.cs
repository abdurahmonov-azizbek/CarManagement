// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class NotFoundCarException : Xeption
    {
        public NotFoundCarException(Guid carId)
            : base(message: $"Couldn't find car with id: {carId}.")
        { }
    }
}
