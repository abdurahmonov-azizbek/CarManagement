// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Addresss.Exceptions
{
    public class AddressDependencyException : Xeption
    {
        public AddressDependencyException(Exception innerException)
            : base(message: "Address dependency error occured, contact support.", innerException)
        { }
    }
}