// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class OfferTypeDependencyValidationException : Xeption
    {
        public OfferTypeDependencyValidationException(Xeption innerException)
            : base(message: "OfferType dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
