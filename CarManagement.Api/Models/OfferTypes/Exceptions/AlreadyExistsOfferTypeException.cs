// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class AlreadyExistsOfferTypeException : Xeption
    {
        public AlreadyExistsOfferTypeException(Exception innerException)
            : base(message: "OfferType already exists.", innerException)
        { }
    }
}
