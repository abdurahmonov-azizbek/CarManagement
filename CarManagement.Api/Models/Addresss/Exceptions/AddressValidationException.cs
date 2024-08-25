// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class AddressValidationException : Xeption
    {
        public AddressValidationException(Xeption innerException)
            : base(message: "Address validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
