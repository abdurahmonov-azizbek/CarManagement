// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class LockedPenaltyException : Xeption
    {
        public LockedPenaltyException(Exception innerException)
            : base(message: "Penalty is locked, please try again.", innerException)
        { }
    }
}
