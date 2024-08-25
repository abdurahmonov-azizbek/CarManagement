// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.DriverLicenses;
using CarManagement.Api.Models.DriverLicenses.Exceptions;

namespace CarManagement.Api.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseService
    {
        private void ValidateDriverLicenseOnAdd(DriverLicense driverLicense)
        {
            ValidateDriverLicenseNotNull(driverLicense);

            Validate(
				(Rule: IsInvalid(driverLicense.Id), Parameter: nameof(DriverLicense.Id)),
				(Rule: IsInvalid(driverLicense.FirstName), Parameter: nameof(DriverLicense.FirstName)),
				(Rule: IsInvalid(driverLicense.LastName), Parameter: nameof(DriverLicense.LastName)),
				(Rule: IsInvalid(driverLicense.MiddleName), Parameter: nameof(DriverLicense.MiddleName)),
				(Rule: IsInvalid(driverLicense.AddressId), Parameter: nameof(DriverLicense.AddressId)),
				(Rule: IsInvalid(driverLicense.CategoryId), Parameter: nameof(DriverLicense.CategoryId)),
				(Rule: IsInvalid(driverLicense.GivenLocation), Parameter: nameof(DriverLicense.GivenLocation)),
				(Rule: IsInvalid(driverLicense.Number), Parameter: nameof(DriverLicense.Number)),
				(Rule: IsInvalid(driverLicense.UserId), Parameter: nameof(DriverLicense.UserId)),
				(Rule: IsInvalid(driverLicense.CreatedDate), Parameter: nameof(DriverLicense.CreatedDate)),
				(Rule: IsInvalid(driverLicense.UpdatedDate), Parameter: nameof(DriverLicense.UpdatedDate)),

                (Rule: IsNotRecent(driverLicense.CreatedDate), Parameter: nameof(DriverLicense.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: driverLicense.CreatedDate,
                    secondDate: driverLicense.UpdatedDate,
                    secondDateName: nameof(DriverLicense.UpdatedDate)),

                    Parameter: nameof(DriverLicense.CreatedDate)));
        }

        private void ValidateDriverLicenseOnModify(DriverLicense driverLicense)
        {
            ValidateDriverLicenseNotNull(driverLicense);

            Validate(
				(Rule: IsInvalid(driverLicense.Id), Parameter: nameof(DriverLicense.Id)),
				(Rule: IsInvalid(driverLicense.FirstName), Parameter: nameof(DriverLicense.FirstName)),
				(Rule: IsInvalid(driverLicense.LastName), Parameter: nameof(DriverLicense.LastName)),
				(Rule: IsInvalid(driverLicense.MiddleName), Parameter: nameof(DriverLicense.MiddleName)),
				(Rule: IsInvalid(driverLicense.AddressId), Parameter: nameof(DriverLicense.AddressId)),
				(Rule: IsInvalid(driverLicense.CategoryId), Parameter: nameof(DriverLicense.CategoryId)),
				(Rule: IsInvalid(driverLicense.GivenLocation), Parameter: nameof(DriverLicense.GivenLocation)),
				(Rule: IsInvalid(driverLicense.Number), Parameter: nameof(DriverLicense.Number)),
				(Rule: IsInvalid(driverLicense.UserId), Parameter: nameof(DriverLicense.UserId)),
				(Rule: IsInvalid(driverLicense.CreatedDate), Parameter: nameof(DriverLicense.CreatedDate)),
				(Rule: IsInvalid(driverLicense.UpdatedDate), Parameter: nameof(DriverLicense.UpdatedDate)),

                (Rule: IsNotRecent(driverLicense.UpdatedDate), Parameter: nameof(DriverLicense.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: driverLicense.UpdatedDate,
                    secondDate: driverLicense.CreatedDate,
                    secondDateName: nameof(driverLicense.CreatedDate)),
                    Parameter: nameof(driverLicense.UpdatedDate)));
        }

        private static void ValidateAgainstStorageDriverLicenseOnModify(DriverLicense inputDriverLicense, DriverLicense storageDriverLicense)
        {
            ValidateStorageDriverLicense(storageDriverLicense, inputDriverLicense.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputDriverLicense.CreatedDate,
                    secondDate: storageDriverLicense.CreatedDate,
                    secondDateName: nameof(DriverLicense.CreatedDate)),
                    Parameter: nameof(DriverLicense.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputDriverLicense.UpdatedDate,
                        secondDate: storageDriverLicense.UpdatedDate,
                        secondDateName: nameof(DriverLicense.UpdatedDate)),
                        Parameter: nameof(DriverLicense.UpdatedDate)));
        }

        private static void ValidateStorageDriverLicense(DriverLicense maybeDriverLicense, Guid driverLicenseId)
        {
            if (maybeDriverLicense is null)
            {
                throw new NotFoundDriverLicenseException(driverLicenseId);
            }
        }

        private void ValidateDriverLicenseId(Guid driverLicenseId) =>
             Validate((Rule: IsInvalid(driverLicenseId), Parameter: nameof(DriverLicense.Id)));

        private void ValidateDriverLicenseNotNull(DriverLicense driverLicense)
        {
            if (driverLicense is null)
            {
                throw new NullDriverLicenseException();
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
            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDriverLicenseException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidDriverLicenseException.ThrowIfContainsErrors();
        }
    }
}
