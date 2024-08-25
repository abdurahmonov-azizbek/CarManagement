// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class PenaltyServiceException : Xeption
    {
        public PenaltyServiceException(Exception innerException)
            : base(message: "Penalty service error occured, contact support.", innerException)
        { }
    }
}