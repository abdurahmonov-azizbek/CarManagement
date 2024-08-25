// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class FailedCategoryStorageException : Xeption
    {
        public FailedCategoryStorageException(Exception innerException)
            : base(message: "Failed category storage error occurred, contact support.", innerException)
        { }
    }
}