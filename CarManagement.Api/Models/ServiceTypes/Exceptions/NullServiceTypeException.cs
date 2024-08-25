// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class NullServiceTypeException : Xeption
    {
        public NullServiceTypeException()
            : base(message: "ServiceType is null.")
        { }
    }
}

