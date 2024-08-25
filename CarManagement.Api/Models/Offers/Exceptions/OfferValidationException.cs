// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class OfferValidationException : Xeption
    {
        public OfferValidationException(Xeption innerException)
            : base(message: "Offer validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
