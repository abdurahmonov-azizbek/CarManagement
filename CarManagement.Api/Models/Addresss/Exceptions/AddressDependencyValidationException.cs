// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class AddressDependencyValidationException : Xeption
    {
        public AddressDependencyValidationException(Xeption innerException)
            : base(message: "Address dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
