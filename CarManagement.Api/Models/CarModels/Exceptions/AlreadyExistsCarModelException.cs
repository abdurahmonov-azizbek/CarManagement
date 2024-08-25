// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class AlreadyExistsCarModelException : Xeption
    {
        public AlreadyExistsCarModelException(Exception innerException)
            : base(message: "CarModel already exists.", innerException)
        { }
    }
}
