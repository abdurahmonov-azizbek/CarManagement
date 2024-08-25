// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class FailedCategoryServiceException : Xeption
    {
        public FailedCategoryServiceException(Exception innerException)
            : base(message: "Failed category service error occurred, please contact support.", innerException)
        { }
    }
}