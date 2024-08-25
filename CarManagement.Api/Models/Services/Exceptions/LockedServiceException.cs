// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class LockedServiceException : Xeption
    {
        public LockedServiceException(Exception innerException)
            : base(message: "Service is locked, please try again.", innerException)
        { }
    }
}
