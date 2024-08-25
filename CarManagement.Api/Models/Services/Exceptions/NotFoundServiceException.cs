// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Services.Exceptions
{
    public class NotFoundServiceException : Xeption
    {
        public NotFoundServiceException(Guid serviceId)
            : base(message: $"Couldn't find service with id: {serviceId}.")
        { }
    }
}
