// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class OfferDependencyValidationException : Xeption
    {
        public OfferDependencyValidationException(Xeption innerException)
            : base(message: "Offer dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
