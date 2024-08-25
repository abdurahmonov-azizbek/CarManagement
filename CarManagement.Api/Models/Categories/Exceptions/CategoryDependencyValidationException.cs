// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class CategoryDependencyValidationException : Xeption
    {
        public CategoryDependencyValidationException(Xeption innerException)
            : base(message: "Category dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
