// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Penalties.Exceptions
{
    public class PenaltyDependencyValidationException : Xeption
    {
        public PenaltyDependencyValidationException(Xeption innerException)
            : base(message: "Penalty dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
