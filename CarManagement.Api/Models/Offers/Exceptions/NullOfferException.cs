// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class NullOfferException : Xeption
    {
        public NullOfferException()
            : base(message: "Offer is null.")
        { }
    }
}

