// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class FailedOfferTypeServiceException : Xeption
    {
        public FailedOfferTypeServiceException(Exception innerException)
            : base(message: "Failed offerType service error occurred, please contact support.", innerException)
        { }
    }
}