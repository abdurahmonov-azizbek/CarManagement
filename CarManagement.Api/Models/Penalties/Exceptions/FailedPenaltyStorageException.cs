// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class FailedPenaltyStorageException : Xeption
    {
        public FailedPenaltyStorageException(Exception innerException)
            : base(message: "Failed penalty storage error occurred, contact support.", innerException)
        { }
    }
}