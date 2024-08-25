// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class NotFoundPenaltyException : Xeption
    {
        public NotFoundPenaltyException(Guid penaltyId)
            : base(message: $"Couldn't find penalty with id: {penaltyId}.")
        { }
    }
}
