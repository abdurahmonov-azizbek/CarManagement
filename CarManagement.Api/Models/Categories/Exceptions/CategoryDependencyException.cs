// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class CategoryDependencyException : Xeption
    {
        public CategoryDependencyException(Exception innerException)
            : base(message: "Category dependency error occured, contact support.", innerException)
        { }
    }
}