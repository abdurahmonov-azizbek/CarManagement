// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class AlreadyExistsCategoryException : Xeption
    {
        public AlreadyExistsCategoryException(Exception innerException)
            : base(message: "Category already exists.", innerException)
        { }
    }
}
