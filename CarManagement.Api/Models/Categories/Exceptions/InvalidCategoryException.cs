// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class InvalidCategoryException : Xeption
    {
        public InvalidCategoryException()
            : base(message: "Category is invalid.")
        { }
    }
}
