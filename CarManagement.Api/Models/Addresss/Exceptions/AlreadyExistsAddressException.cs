// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class AlreadyExistsAddressException : Xeption
    {
        public AlreadyExistsAddressException(Exception innerException)
            : base(message: "Address already exists.", innerException)
        { }
    }
}
