// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class InvalidPenaltyException : Xeption
    {
        public InvalidPenaltyException()
            : base(message: "Penalty is invalid.")
        { }
    }
}
