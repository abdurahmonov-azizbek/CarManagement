// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class AlreadyExistsCarTypeException : Xeption
    {
        public AlreadyExistsCarTypeException(Exception innerException)
            : base(message: "CarType already exists.", innerException)
        { }
    }
}
