// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.OfferTypes.Exceptions
{
    public class OfferTypeDependencyException : Xeption
    {
        public OfferTypeDependencyException(Exception innerException)
            : base(message: "OfferType dependency error occured, contact support.", innerException)
        { }
    }
}