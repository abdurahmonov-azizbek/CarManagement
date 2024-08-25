// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class OfferTypeValidationException : Xeption
    {
        public OfferTypeValidationException(Xeption innerException)
            : base(message: "OfferType validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
