// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class AlreadyExistsOfferException : Xeption
    {
        public AlreadyExistsOfferException(Exception innerException)
            : base(message: "Offer already exists.", innerException)
        { }
    }
}
