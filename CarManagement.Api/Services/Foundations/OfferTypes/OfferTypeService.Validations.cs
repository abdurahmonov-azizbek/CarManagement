// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;

namespace CarManagement.Api.Services.Foundations.OfferTypes
{
    public partial class OfferTypeService
    {
        private void ValidateOfferTypeOnAdd(OfferType offerType)
        {
            ValidateOfferTypeNotNull(offerType);

            Validate(
				(Rule: IsInvalid(offerType.Id), Parameter: nameof(OfferType.Id)),
				(Rule: IsInvalid(offerType.Name), Parameter: nameof(OfferType.Name)),
				(Rule: IsInvalid(offerType.CreatedDate), Parameter: nameof(OfferType.CreatedDate)),
				(Rule: IsInvalid(offerType.UpdatedDate), Parameter: nameof(OfferType.UpdatedDate)),

                (Rule: IsNotRecent(offerType.CreatedDate), Parameter: nameof(OfferType.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: offerType.CreatedDate,
                    secondDate: offerType.UpdatedDate,
                    secondDateName: nameof(OfferType.UpdatedDate)),

                    Parameter: nameof(OfferType.CreatedDate)));
        }

        private void ValidateOfferTypeOnModify(OfferType offerType)
        {
            ValidateOfferTypeNotNull(offerType);

            Validate(
				(Rule: IsInvalid(offerType.Id), Parameter: nameof(OfferType.Id)),
				(Rule: IsInvalid(offerType.Name), Parameter: nameof(OfferType.Name)),
				(Rule: IsInvalid(offerType.CreatedDate), Parameter: nameof(OfferType.CreatedDate)),
				(Rule: IsInvalid(offerType.UpdatedDate), Parameter: nameof(OfferType.UpdatedDate)),

                (Rule: IsNotRecent(offerType.UpdatedDate), Parameter: nameof(OfferType.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: offerType.UpdatedDate,
                    secondDate: offerType.CreatedDate,
                    secondDateName: nameof(offerType.CreatedDate)),
                    Parameter: nameof(offerType.UpdatedDate)));
        }

        private static void ValidateAgainstStorageOfferTypeOnModify(OfferType inputOfferType, OfferType storageOfferType)
        {
            ValidateStorageOfferType(storageOfferType, inputOfferType.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputOfferType.CreatedDate,
                    secondDate: storageOfferType.CreatedDate,
                    secondDateName: nameof(OfferType.CreatedDate)),
                    Parameter: nameof(OfferType.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputOfferType.UpdatedDate,
                        secondDate: storageOfferType.UpdatedDate,
                        secondDateName: nameof(OfferType.UpdatedDate)),
                        Parameter: nameof(OfferType.UpdatedDate)));
        }

        private static void ValidateStorageOfferType(OfferType maybeOfferType, Guid offerTypeId)
        {
            if (maybeOfferType is null)
            {
                throw new NotFoundOfferTypeException(offerTypeId);
            }
        }

        private void ValidateOfferTypeId(Guid offerTypeId) =>
             Validate((Rule: IsInvalid(offerTypeId), Parameter: nameof(OfferType.Id)));

        private void ValidateOfferTypeNotNull(OfferType offerType)
        {
            if (offerType is null)
            {
                throw new NullOfferTypeException();
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
            var invalidOfferTypeException = new InvalidOfferTypeException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidOfferTypeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidOfferTypeException.ThrowIfContainsErrors();
        }
    }
}
