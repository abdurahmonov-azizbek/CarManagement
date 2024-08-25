// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace CarManagement.Api.Models.Categories.Exceptions
{
    public class NotFoundCategoryException : Xeption
    {
        public NotFoundCategoryException(Guid categoryId)
            : base(message: $"Couldn't find category with id: {categoryId}.")
        { }
    }
}
