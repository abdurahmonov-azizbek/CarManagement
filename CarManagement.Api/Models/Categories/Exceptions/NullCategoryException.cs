// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class NullCategoryException : Xeption
    {
        public NullCategoryException()
            : base(message: "Category is null.")
        { }
    }
}

