// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.CarModels;
using CarManagement.Api.Models.CarModels.Exceptions;

namespace CarManagement.Api.Services.Foundations.CarModels
{
    public partial class CarModelService
    {
        private void ValidateCarModelOnAdd(CarModel carModel)
        {
            ValidateCarModelNotNull(carModel);

            Validate(
				(Rule: IsInvalid(carModel.Id), Parameter: nameof(CarModel.Id)),
				(Rule: IsInvalid(carModel.Name), Parameter: nameof(CarModel.Name)),
				(Rule: IsInvalid(carModel.CreatedDate), Parameter: nameof(CarModel.CreatedDate)),
				(Rule: IsInvalid(carModel.UpdatedDate), Parameter: nameof(CarModel.UpdatedDate)),

                (Rule: IsNotRecent(carModel.CreatedDate), Parameter: nameof(CarModel.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: carModel.CreatedDate,
                    secondDate: carModel.UpdatedDate,
                    secondDateName: nameof(CarModel.UpdatedDate)),

                    Parameter: nameof(CarModel.CreatedDate)));
        }

        private void ValidateCarModelOnModify(CarModel carModel)
        {
            ValidateCarModelNotNull(carModel);

            Validate(
				(Rule: IsInvalid(carModel.Id), Parameter: nameof(CarModel.Id)),
				(Rule: IsInvalid(carModel.Name), Parameter: nameof(CarModel.Name)),
				(Rule: IsInvalid(carModel.CreatedDate), Parameter: nameof(CarModel.CreatedDate)),
				(Rule: IsInvalid(carModel.UpdatedDate), Parameter: nameof(CarModel.UpdatedDate)),

                (Rule: IsNotRecent(carModel.UpdatedDate), Parameter: nameof(CarModel.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: carModel.UpdatedDate,
                    secondDate: carModel.CreatedDate,
                    secondDateName: nameof(carModel.CreatedDate)),
                    Parameter: nameof(carModel.UpdatedDate)));
        }

        private static void ValidateAgainstStorageCarModelOnModify(CarModel inputCarModel, CarModel storageCarModel)
        {
            ValidateStorageCarModel(storageCarModel, inputCarModel.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputCarModel.CreatedDate,
                    secondDate: storageCarModel.CreatedDate,
                    secondDateName: nameof(CarModel.CreatedDate)),
                    Parameter: nameof(CarModel.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputCarModel.UpdatedDate,
                        secondDate: storageCarModel.UpdatedDate,
                        secondDateName: nameof(CarModel.UpdatedDate)),
                        Parameter: nameof(CarModel.UpdatedDate)));
        }

        private static void ValidateStorageCarModel(CarModel maybeCarModel, Guid carModelId)
        {
            if (maybeCarModel is null)
            {
                throw new NotFoundCarModelException(carModelId);
            }
        }

        private void ValidateCarModelId(Guid carModelId) =>
             Validate((Rule: IsInvalid(carModelId), Parameter: nameof(CarModel.Id)));

        private void ValidateCarModelNotNull(CarModel carModel)
        {
            if (carModel is null)
            {
                throw new NullCarModelException();
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
            var invalidCarModelException = new InvalidCarModelException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCarModelException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidCarModelException.ThrowIfContainsErrors();
        }
    }
}
