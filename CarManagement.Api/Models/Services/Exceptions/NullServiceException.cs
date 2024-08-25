// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class NullServiceException : Xeption
    {
        public NullServiceException()
            : base(message: "Service is null.")
        { }
    }
}

