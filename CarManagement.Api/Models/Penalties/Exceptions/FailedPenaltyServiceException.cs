// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class FailedPenaltyServiceException : Xeption
    {
        public FailedPenaltyServiceException(Exception innerException)
            : base(message: "Failed penalty service error occurred, please contact support.", innerException)
        { }
    }
}