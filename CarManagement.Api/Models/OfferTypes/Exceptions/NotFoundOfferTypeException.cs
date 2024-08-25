// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class NotFoundOfferTypeException : Xeption
    {
        public NotFoundOfferTypeException(Guid offerTypeId)
            : base(message: $"Couldn't find offerType with id: {offerTypeId}.")
        { }
    }
}
