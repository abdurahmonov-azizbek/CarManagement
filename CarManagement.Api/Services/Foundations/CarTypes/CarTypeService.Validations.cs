// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;

namespace CarManagement.Api.Services.Foundations.CarTypes
{
    public partial class CarTypeService
    {
        private void ValidateCarTypeOnAdd(CarType carType)
        {
            ValidateCarTypeNotNull(carType);

            Validate(
				(Rule: IsInvalid(carType.Id), Parameter: nameof(CarType.Id)),
				(Rule: IsInvalid(carType.Name), Parameter: nameof(CarType.Name)),
				(Rule: IsInvalid(carType.CreatedDate), Parameter: nameof(CarType.CreatedDate)),
				(Rule: IsInvalid(carType.UpdatedDate), Parameter: nameof(CarType.UpdatedDate)),

                (Rule: IsNotRecent(carType.CreatedDate), Parameter: nameof(CarType.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: carType.CreatedDate,
                    secondDate: carType.UpdatedDate,
                    secondDateName: nameof(CarType.UpdatedDate)),

                    Parameter: nameof(CarType.CreatedDate)));
        }

        private void ValidateCarTypeOnModify(CarType carType)
        {
            ValidateCarTypeNotNull(carType);

            Validate(
				(Rule: IsInvalid(carType.Id), Parameter: nameof(CarType.Id)),
				(Rule: IsInvalid(carType.Name), Parameter: nameof(CarType.Name)),
				(Rule: IsInvalid(carType.CreatedDate), Parameter: nameof(CarType.CreatedDate)),
				(Rule: IsInvalid(carType.UpdatedDate), Parameter: nameof(CarType.UpdatedDate)),

                (Rule: IsNotRecent(carType.UpdatedDate), Parameter: nameof(CarType.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: carType.UpdatedDate,
                    secondDate: carType.CreatedDate,
                    secondDateName: nameof(carType.CreatedDate)),
                    Parameter: nameof(carType.UpdatedDate)));
        }

        private static void ValidateAgainstStorageCarTypeOnModify(CarType inputCarType, CarType storageCarType)
        {
            ValidateStorageCarType(storageCarType, inputCarType.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputCarType.CreatedDate,
                    secondDate: storageCarType.CreatedDate,
                    secondDateName: nameof(CarType.CreatedDate)),
                    Parameter: nameof(CarType.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputCarType.UpdatedDate,
                        secondDate: storageCarType.UpdatedDate,
                        secondDateName: nameof(CarType.UpdatedDate)),
                        Parameter: nameof(CarType.UpdatedDate)));
        }

        private static void ValidateStorageCarType(CarType maybeCarType, Guid carTypeId)
        {
            if (maybeCarType is null)
            {
                throw new NotFoundCarTypeException(carTypeId);
            }
        }

        private void ValidateCarTypeId(Guid carTypeId) =>
             Validate((Rule: IsInvalid(carTypeId), Parameter: nameof(CarType.Id)));

        private void ValidateCarTypeNotNull(CarType carType)
        {
            if (carType is null)
            {
                throw new NullCarTypeException();
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
            var invalidCarTypeException = new InvalidCarTypeException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCarTypeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidCarTypeException.ThrowIfContainsErrors();
        }
    }
}
