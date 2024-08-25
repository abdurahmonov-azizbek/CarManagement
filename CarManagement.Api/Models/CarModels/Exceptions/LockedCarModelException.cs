// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.CarModels.Exceptions
{
    public class LockedCarModelException : Xeption
    {
        public LockedCarModelException(Exception innerException)
            : base(message: "CarModel is locked, please try again.", innerException)
        { }
    }
}
