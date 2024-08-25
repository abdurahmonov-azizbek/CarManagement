// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Addresss;
using CarManagement.Api.Models.Addresss.Exceptions;

namespace CarManagement.Api.Services.Foundations.Addresss
{
    public partial class AddressService
    {
        private void ValidateAddressOnAdd(Address address)
        {
            ValidateAddressNotNull(address);

            Validate(
				(Rule: IsInvalid(address.Id), Parameter: nameof(Address.Id)),
				(Rule: IsInvalid(address.Country), Parameter: nameof(Address.Country)),
				(Rule: IsInvalid(address.Region), Parameter: nameof(Address.Region)),
				(Rule: IsInvalid(address.Street), Parameter: nameof(Address.Street)),
				(Rule: IsInvalid(address.HomeNumber), Parameter: nameof(Address.HomeNumber)),
				(Rule: IsInvalid(address.CreatedDate), Parameter: nameof(Address.CreatedDate)),
				(Rule: IsInvalid(address.UpdatedDate), Parameter: nameof(Address.UpdatedDate)),

                (Rule: IsNotRecent(address.CreatedDate), Parameter: nameof(Address.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: address.CreatedDate,
                    secondDate: address.UpdatedDate,
                    secondDateName: nameof(Address.UpdatedDate)),

                    Parameter: nameof(Address.CreatedDate)));
        }

        private void ValidateAddressOnModify(Address address)
        {
            ValidateAddressNotNull(address);

            Validate(
				(Rule: IsInvalid(address.Id), Parameter: nameof(Address.Id)),
				(Rule: IsInvalid(address.Country), Parameter: nameof(Address.Country)),
				(Rule: IsInvalid(address.Region), Parameter: nameof(Address.Region)),
				(Rule: IsInvalid(address.Street), Parameter: nameof(Address.Street)),
				(Rule: IsInvalid(address.HomeNumber), Parameter: nameof(Address.HomeNumber)),
				(Rule: IsInvalid(address.CreatedDate), Parameter: nameof(Address.CreatedDate)),
				(Rule: IsInvalid(address.UpdatedDate), Parameter: nameof(Address.UpdatedDate)),

                (Rule: IsNotRecent(address.UpdatedDate), Parameter: nameof(Address.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: address.UpdatedDate,
                    secondDate: address.CreatedDate,
                    secondDateName: nameof(address.CreatedDate)),
                    Parameter: nameof(address.UpdatedDate)));
        }

        private static void ValidateAgainstStorageAddressOnModify(Address inputAddress, Address storageAddress)
        {
            ValidateStorageAddress(storageAddress, inputAddress.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputAddress.CreatedDate,
                    secondDate: storageAddress.CreatedDate,
                    secondDateName: nameof(Address.CreatedDate)),
                    Parameter: nameof(Address.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputAddress.UpdatedDate,
                        secondDate: storageAddress.UpdatedDate,
                        secondDateName: nameof(Address.UpdatedDate)),
                        Parameter: nameof(Address.UpdatedDate)));
        }

        private static void ValidateStorageAddress(Address maybeAddress, Guid addressId)
        {
            if (maybeAddress is null)
            {
                throw new NotFoundAddressException(addressId);
            }
        }

        private void ValidateAddressId(Guid addressId) =>
             Validate((Rule: IsInvalid(addressId), Parameter: nameof(Address.Id)));

        private void ValidateAddressNotNull(Address address)
        {
            if (address is null)
            {
                throw new NullAddressException();
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
            var invalidAddressException = new InvalidAddressException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidAddressException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidAddressException.ThrowIfContainsErrors();
        }
    }
}
