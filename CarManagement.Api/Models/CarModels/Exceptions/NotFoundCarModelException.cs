// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class NotFoundCarModelException : Xeption
    {
        public NotFoundCarModelException(Guid carModelId)
            : base(message: $"Couldn't find carModel with id: {carModelId}.")
        { }
    }
}
