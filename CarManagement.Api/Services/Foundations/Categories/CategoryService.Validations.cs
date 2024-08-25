// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Categories;
using CarManagement.Api.Models.Categories.Exceptions;

namespace CarManagement.Api.Services.Foundations.Categories
{
    public partial class CategoryService
    {
        private void ValidateCategoryOnAdd(Category category)
        {
            ValidateCategoryNotNull(category);

            Validate(
				(Rule: IsInvalid(category.Id), Parameter: nameof(Category.Id)),
				(Rule: IsInvalid(category.Name), Parameter: nameof(Category.Name)),
				(Rule: IsInvalid(category.CreatedDate), Parameter: nameof(Category.CreatedDate)),
				(Rule: IsInvalid(category.UpdatedDate), Parameter: nameof(Category.UpdatedDate)),

                (Rule: IsNotRecent(category.CreatedDate), Parameter: nameof(Category.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: category.CreatedDate,
                    secondDate: category.UpdatedDate,
                    secondDateName: nameof(Category.UpdatedDate)),

                    Parameter: nameof(Category.CreatedDate)));
        }

        private void ValidateCategoryOnModify(Category category)
        {
            ValidateCategoryNotNull(category);

            Validate(
				(Rule: IsInvalid(category.Id), Parameter: nameof(Category.Id)),
				(Rule: IsInvalid(category.Name), Parameter: nameof(Category.Name)),
				(Rule: IsInvalid(category.CreatedDate), Parameter: nameof(Category.CreatedDate)),
				(Rule: IsInvalid(category.UpdatedDate), Parameter: nameof(Category.UpdatedDate)),

                (Rule: IsNotRecent(category.UpdatedDate), Parameter: nameof(Category.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: category.UpdatedDate,
                    secondDate: category.CreatedDate,
                    secondDateName: nameof(category.CreatedDate)),
                    Parameter: nameof(category.UpdatedDate)));
        }

        private static void ValidateAgainstStorageCategoryOnModify(Category inputCategory, Category storageCategory)
        {
            ValidateStorageCategory(storageCategory, inputCategory.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputCategory.CreatedDate,
                    secondDate: storageCategory.CreatedDate,
                    secondDateName: nameof(Category.CreatedDate)),
                    Parameter: nameof(Category.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputCategory.UpdatedDate,
                        secondDate: storageCategory.UpdatedDate,
                        secondDateName: nameof(Category.UpdatedDate)),
                        Parameter: nameof(Category.UpdatedDate)));
        }

        private static void ValidateStorageCategory(Category maybeCategory, Guid categoryId)
        {
            if (maybeCategory is null)
            {
                throw new NotFoundCategoryException(categoryId);
            }
        }

        private void ValidateCategoryId(Guid categoryId) =>
             Validate((Rule: IsInvalid(categoryId), Parameter: nameof(Category.Id)));

        private void ValidateCategoryNotNull(Category category)
        {
            if (category is null)
            {
                throw new NullCategoryException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsInvalid<T>(T value) => new
        {
            Condition = IsEnumInvalid(value),
            Message = "Value is not recognized"
        };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private static bool IsEnumInvalid<T>(T value)
        {
            bool isDefined = Enum.IsDefined(typeof(T), value);

            return isDefined is false;
        }

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDate = this.dateTimeBroker.GetCurrentDateTimeOffset();
            TimeSpan timeDifference = currentDate.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidCategoryException = new InvalidCategoryException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCategoryException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidCategoryException.ThrowIfContainsErrors();
        }
    }
}
