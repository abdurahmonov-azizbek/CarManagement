// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class NullOfferTypeException : Xeption
    {
        public NullOfferTypeException()
            : base(message: "OfferType is null.")
        { }
    }
}

