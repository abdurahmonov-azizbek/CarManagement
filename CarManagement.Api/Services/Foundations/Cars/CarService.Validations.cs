// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Cars;
using CarManagement.Api.Models.Cars.Exceptions;

namespace CarManagement.Api.Services.Foundations.Cars
{
    public partial class CarService
    {
        private void ValidateCarOnAdd(Car car)
        {
            ValidateCarNotNull(car);

            Validate(
				(Rule: IsInvalid(car.Id), Parameter: nameof(Car.Id)),
				(Rule: IsInvalid(car.TypeId), Parameter: nameof(Car.TypeId)),
				(Rule: IsInvalid(car.ModelId), Parameter: nameof(Car.ModelId)),
				(Rule: IsInvalid(car.Color), Parameter: nameof(Car.Color)),
				(Rule: IsInvalid(car.Number), Parameter: nameof(Car.Number)),
				(Rule: IsInvalid(car.TexPassportNumber), Parameter: nameof(Car.TexPassportNumber)),
				(Rule: IsInvalid(car.UserId), Parameter: nameof(Car.UserId)),
				(Rule: IsInvalid(car.CreatedDate), Parameter: nameof(Car.CreatedDate)),
				(Rule: IsInvalid(car.UpdatedDate), Parameter: nameof(Car.UpdatedDate)),

                (Rule: IsNotRecent(car.CreatedDate), Parameter: nameof(Car.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: car.CreatedDate,
                    secondDate: car.UpdatedDate,
                    secondDateName: nameof(Car.UpdatedDate)),

                    Parameter: nameof(Car.CreatedDate)));
        }

        private void ValidateCarOnModify(Car car)
        {
            ValidateCarNotNull(car);

            Validate(
				(Rule: IsInvalid(car.Id), Parameter: nameof(Car.Id)),
				(Rule: IsInvalid(car.TypeId), Parameter: nameof(Car.TypeId)),
				(Rule: IsInvalid(car.ModelId), Parameter: nameof(Car.ModelId)),
				(Rule: IsInvalid(car.Color), Parameter: nameof(Car.Color)),
				(Rule: IsInvalid(car.Number), Parameter: nameof(Car.Number)),
				(Rule: IsInvalid(car.TexPassportNumber), Parameter: nameof(Car.TexPassportNumber)),
				(Rule: IsInvalid(car.UserId), Parameter: nameof(Car.UserId)),
				(Rule: IsInvalid(car.CreatedDate), Parameter: nameof(Car.CreatedDate)),
				(Rule: IsInvalid(car.UpdatedDate), Parameter: nameof(Car.UpdatedDate)),

                (Rule: IsNotRecent(car.UpdatedDate), Parameter: nameof(Car.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: car.UpdatedDate,
                    secondDate: car.CreatedDate,
                    secondDateName: nameof(car.CreatedDate)),
                    Parameter: nameof(car.UpdatedDate)));
        }

        private static void ValidateAgainstStorageCarOnModify(Car inputCar, Car storageCar)
        {
            ValidateStorageCar(storageCar, inputCar.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputCar.CreatedDate,
                    secondDate: storageCar.CreatedDate,
                    secondDateName: nameof(Car.CreatedDate)),
                    Parameter: nameof(Car.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputCar.UpdatedDate,
                        secondDate: storageCar.UpdatedDate,
                        secondDateName: nameof(Car.UpdatedDate)),
                        Parameter: nameof(Car.UpdatedDate)));
        }

        private static void ValidateStorageCar(Car maybeCar, Guid carId)
        {
            if (maybeCar is null)
            {
                throw new NotFoundCarException(carId);
            }
        }

        private void ValidateCarId(Guid carId) =>
             Validate((Rule: IsInvalid(carId), Parameter: nameof(Car.Id)));

        private void ValidateCarNotNull(Car car)
        {
            if (car is null)
            {
                throw new NullCarException();
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
            var invalidCarException = new InvalidCarException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCarException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidCarException.ThrowIfContainsErrors();
        }
    }
}
