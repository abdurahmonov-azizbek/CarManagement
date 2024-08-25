// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.ServiceTypes.Exceptions
{
    public class InvalidServiceTypeException : Xeption
    {
        public InvalidServiceTypeException()
            : base(message: "ServiceType is invalid.")
        { }
    }
}
