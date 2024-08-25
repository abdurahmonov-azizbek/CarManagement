// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class FailedAddressStorageException : Xeption
    {
        public FailedAddressStorageException(Exception innerException)
            : base(message: "Failed address storage error occurred, contact support.", innerException)
        { }
    }
}