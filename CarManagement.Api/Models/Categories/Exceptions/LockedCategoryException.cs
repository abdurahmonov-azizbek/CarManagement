// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class LockedCategoryException : Xeption
    {
        public LockedCategoryException(Exception innerException)
            : base(message: "Category is locked, please try again.", innerException)
        { }
    }
}
