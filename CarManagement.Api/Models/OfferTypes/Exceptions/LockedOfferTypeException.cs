// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class LockedOfferTypeException : Xeption
    {
        public LockedOfferTypeException(Exception innerException)
            : base(message: "OfferType is locked, please try again.", innerException)
        { }
    }
}
