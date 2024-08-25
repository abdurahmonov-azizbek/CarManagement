// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class PenaltyDependencyException : Xeption
    {
        public PenaltyDependencyException(Exception innerException)
            : base(message: "Penalty dependency error occured, contact support.", innerException)
        { }
    }
}