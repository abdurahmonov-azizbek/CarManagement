// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class NullAddressException : Xeption
    {
        public NullAddressException()
            : base(message: "Address is null.")
        { }
    }
}

