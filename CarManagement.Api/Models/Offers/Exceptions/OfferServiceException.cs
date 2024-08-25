// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class OfferServiceException : Xeption
    {
        public OfferServiceException(Exception innerException)
            : base(message: "Offer service error occured, contact support.", innerException)
        { }
    }
}