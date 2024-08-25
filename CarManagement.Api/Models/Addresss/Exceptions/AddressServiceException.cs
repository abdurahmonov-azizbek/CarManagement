// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class AddressServiceException : Xeption
    {
        public AddressServiceException(Exception innerException)
            : base(message: "Address service error occured, contact support.", innerException)
        { }
    }
}