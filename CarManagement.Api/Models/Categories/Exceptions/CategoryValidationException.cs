// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class CategoryValidationException : Xeption
    {
        public CategoryValidationException(Xeption innerException)
            : base(message: "Category validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
