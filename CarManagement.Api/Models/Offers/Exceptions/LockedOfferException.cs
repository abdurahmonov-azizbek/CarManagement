// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class LockedOfferException : Xeption
    {
        public LockedOfferException(Exception innerException)
            : base(message: "Offer is locked, please try again.", innerException)
        { }
    }
}
