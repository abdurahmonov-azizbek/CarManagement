// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class CategoryServiceException : Xeption
    {
        public CategoryServiceException(Exception innerException)
            : base(message: "Category service error occured, contact support.", innerException)
        { }
    }
}