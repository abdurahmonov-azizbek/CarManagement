// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class NotFoundAddressException : Xeption
    {
        public NotFoundAddressException(Guid addressId)
            : base(message: $"Couldn't find address with id: {addressId}.")
        { }
    }
}
