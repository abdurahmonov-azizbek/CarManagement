// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class FailedAddressServiceException : Xeption
    {
        public FailedAddressServiceException(Exception innerException)
            : base(message: "Failed address service error occurred, please contact support.", innerException)
        { }
    }
}