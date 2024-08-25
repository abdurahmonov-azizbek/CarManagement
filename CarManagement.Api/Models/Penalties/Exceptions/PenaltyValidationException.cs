// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class PenaltyValidationException : Xeption
    {
        public PenaltyValidationException(Xeption innerException)
            : base(message: "Penalty validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
