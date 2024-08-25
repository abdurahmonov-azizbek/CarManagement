// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class NotFoundOfferException : Xeption
    {
        public NotFoundOfferException(Guid offerId)
            : base(message: $"Couldn't find offer with id: {offerId}.")
        { }
    }
}
