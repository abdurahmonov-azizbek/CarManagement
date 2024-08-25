// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class AlreadyExistsServiceTypeException : Xeption
    {
        public AlreadyExistsServiceTypeException(Exception innerException)
            : base(message: "ServiceType already exists.", innerException)
        { }
    }
}
