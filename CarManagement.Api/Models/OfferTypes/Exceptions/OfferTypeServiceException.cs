// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class OfferTypeServiceException : Xeption
    {
        public OfferTypeServiceException(Exception innerException)
            : base(message: "OfferType service error occured, contact support.", innerException)
        { }
    }
}