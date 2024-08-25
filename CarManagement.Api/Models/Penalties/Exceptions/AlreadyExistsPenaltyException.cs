// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class AlreadyExistsPenaltyException : Xeption
    {
        public AlreadyExistsPenaltyException(Exception innerException)
            : base(message: "Penalty already exists.", innerException)
        { }
    }
}
