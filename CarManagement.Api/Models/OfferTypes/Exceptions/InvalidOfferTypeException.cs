// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class InvalidOfferTypeException : Xeption
    {
        public InvalidOfferTypeException()
            : base(message: "OfferType is invalid.")
        { }
    }
}
