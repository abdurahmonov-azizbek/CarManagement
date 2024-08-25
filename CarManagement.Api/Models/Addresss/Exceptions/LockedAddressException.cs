// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class LockedAddressException : Xeption
    {
        public LockedAddressException(Exception innerException)
            : base(message: "Address is locked, please try again.", innerException)
        { }
    }
}
