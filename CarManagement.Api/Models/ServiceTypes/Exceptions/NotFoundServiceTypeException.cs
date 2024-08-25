// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class NotFoundServiceTypeException : Xeption
    {
        public NotFoundServiceTypeException(Guid serviceTypeId)
            : base(message: $"Couldn't find serviceType with id: {serviceTypeId}.")
        { }
    }
}
