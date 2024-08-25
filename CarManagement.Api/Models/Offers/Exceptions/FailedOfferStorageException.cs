// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Offers.Exceptions
{
    public class FailedOfferStorageException : Xeption
    {
        public FailedOfferStorageException(Exception innerException)
            : base(message: "Failed offer storage error occurred, contact support.", innerException)
        { }
    }
}