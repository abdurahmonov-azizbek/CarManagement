// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class InvalidOfferException : Xeption
    {
        public InvalidOfferException()
            : base(message: "Offer is invalid.")
        { }
    }
}
