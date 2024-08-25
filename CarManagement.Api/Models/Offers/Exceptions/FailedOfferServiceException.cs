// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class FailedOfferServiceException : Xeption
    {
        public FailedOfferServiceException(Exception innerException)
            : base(message: "Failed offer service error occurred, please contact support.", innerException)
        { }
    }
}