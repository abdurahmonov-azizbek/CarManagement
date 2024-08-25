// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class AlreadyExistsServiceException : Xeption
    {
        public AlreadyExistsServiceException(Exception innerException)
            : base(message: "Service already exists.", innerException)
        { }
    }
}
