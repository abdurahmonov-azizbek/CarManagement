// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarTypes.Exceptions
{
    public class LockedCarTypeException : Xeption
    {
        public LockedCarTypeException(Exception innerException)
            : base(message: "CarType is locked, please try again.", innerException)
        { }
    }
}
