// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class LockedServiceTypeException : Xeption
    {
        public LockedServiceTypeException(Exception innerException)
            : base(message: "ServiceType is locked, please try again.", innerException)
        { }
    }
}
