// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class OfferDependencyException : Xeption
    {
        public OfferDependencyException(Exception innerException)
            : base(message: "Offer dependency error occured, contact support.", innerException)
        { }
    }
}