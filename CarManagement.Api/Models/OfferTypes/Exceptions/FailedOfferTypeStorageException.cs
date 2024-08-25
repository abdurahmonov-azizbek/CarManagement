// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class FailedOfferTypeStorageException : Xeption
    {
        public FailedOfferTypeStorageException(Exception innerException)
            : base(message: "Failed offerType storage error occurred, contact support.", innerException)
        { }
    }
}