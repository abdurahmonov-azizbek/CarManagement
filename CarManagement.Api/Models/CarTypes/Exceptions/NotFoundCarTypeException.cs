// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class NotFoundCarTypeException : Xeption
    {
        public NotFoundCarTypeException(Guid carTypeId)
            : base(message: $"Couldn't find carType with id: {carTypeId}.")
        { }
    }
}
