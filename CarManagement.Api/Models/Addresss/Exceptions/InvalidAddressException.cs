// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class InvalidAddressException : Xeption
    {
        public InvalidAddressException()
            : base(message: "Address is invalid.")
        { }
    }
}
