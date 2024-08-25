// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class NullPenaltyException : Xeption
    {
        public NullPenaltyException()
            : base(message: "Penalty is null.")
        { }
    }
}

