// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.ServiceTypes;
using CarManagement.Api.Models.ServiceTypes.Exceptions;

namespace CarManagement.Api.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeService
    {
        private void ValidateServiceTypeOnAdd(ServiceType serviceType)
        {
            ValidateServiceTypeNotNull(serviceType);

            Validate(
				(Rule: IsInvalid(serviceType.Id), Parameter: nameof(ServiceType.Id)),
				(Rule: IsInvalid(serviceType.Name), Parameter: nameof(ServiceType.Name)),
				(Rule: IsInvalid(serviceType.CreatedDate), Parameter: nameof(ServiceType.CreatedDate)),
				(Rule: IsInvalid(serviceType.UpdatedDate), Parameter: nameof(ServiceType.UpdatedDate)),

                (Rule: IsNotRecent(serviceType.CreatedDate), Parameter: nameof(ServiceType.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: serviceType.CreatedDate,
                    secondDate: serviceType.UpdatedDate,
                    secondDateName: nameof(ServiceType.UpdatedDate)),

                    Parameter: nameof(ServiceType.CreatedDate)));
        }

        private void ValidateServiceTypeOnModify(ServiceType serviceType)
        {
            ValidateServiceTypeNotNull(serviceType);

            Validate(
				(Rule: IsInvalid(serviceType.Id), Parameter: nameof(ServiceType.Id)),
				(Rule: IsInvalid(serviceType.Name), Parameter: nameof(ServiceType.Name)),
				(Rule: IsInvalid(serviceType.CreatedDate), Parameter: nameof(ServiceType.CreatedDate)),
				(Rule: IsInvalid(serviceType.UpdatedDate), Parameter: nameof(ServiceType.UpdatedDate)),

                (Rule: IsNotRecent(serviceType.UpdatedDate), Parameter: nameof(ServiceType.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: serviceType.UpdatedDate,
                    secondDate: serviceType.CreatedDate,
                    secondDateName: nameof(serviceType.CreatedDate)),
                    Parameter: nameof(serviceType.UpdatedDate)));
        }

        private static void ValidateAgainstStorageServiceTypeOnModify(ServiceType inputServiceType, ServiceType storageServiceType)
        {
            ValidateStorageServiceType(storageServiceType, inputServiceType.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputServiceType.CreatedDate,
                    secondDate: storageServiceType.CreatedDate,
                    secondDateName: nameof(ServiceType.CreatedDate)),
                    Parameter: nameof(ServiceType.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputServiceType.UpdatedDate,
                        secondDate: storageServiceType.UpdatedDate,
                        secondDateName: nameof(ServiceType.UpdatedDate)),
                        Parameter: nameof(ServiceType.UpdatedDate)));
        }

        private static void ValidateStorageServiceType(ServiceType maybeServiceType, Guid serviceTypeId)
        {
            if (maybeServiceType is null)
            {
                throw new NotFoundServiceTypeException(serviceTypeId);
            }
        }

        private void ValidateServiceTypeId(Guid serviceTypeId) =>
             Validate((Rule: IsInvalid(serviceTypeId), Parameter: nameof(ServiceType.Id)));

        private void ValidateServiceTypeNotNull(ServiceType serviceType)
        {
            if (serviceType is null)
            {
                throw new NullServiceTypeException();
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
            var invalidServiceTypeException = new InvalidServiceTypeException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidServiceTypeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidServiceTypeException.ThrowIfContainsErrors();
        }
    }
}
