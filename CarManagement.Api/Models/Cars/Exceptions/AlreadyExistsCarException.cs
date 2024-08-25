// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Cars.Exceptions
{
    public class AlreadyExistsCarException : Xeption
    {
        public AlreadyExistsCarException(Exception innerException)
            : base(message: "Car already exists.", innerException)
        { }
    }
}
