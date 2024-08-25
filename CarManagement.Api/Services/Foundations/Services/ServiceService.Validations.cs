// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Services;
using CarManagement.Api.Models.Services.Exceptions;

namespace CarManagement.Api.Services.Foundations.Services
{
    public partial class ServiceService
    {
        private void ValidateServiceOnAdd(Service service)
        {
            ValidateServiceNotNull(service);

            Validate(
				(Rule: IsInvalid(service.Id), Parameter: nameof(Service.Id)),
				(Rule: IsInvalid(service.TypeId), Parameter: nameof(Service.TypeId)),
				(Rule: IsInvalid(service.Name), Parameter: nameof(Service.Name)),
				(Rule: IsInvalid(service.SertificateNumber), Parameter: nameof(Service.SertificateNumber)),
				(Rule: IsInvalid(service.OwnerFIO), Parameter: nameof(Service.OwnerFIO)),
				(Rule: IsInvalid(service.Address), Parameter: nameof(Service.Address)),
				(Rule: IsInvalid(service.PhoneNumber), Parameter: nameof(Service.PhoneNumber)),
				(Rule: IsInvalid(service.CreatedDate), Parameter: nameof(Service.CreatedDate)),
				(Rule: IsInvalid(service.UpdatedDate), Parameter: nameof(Service.UpdatedDate)),

                (Rule: IsNotRecent(service.CreatedDate), Parameter: nameof(Service.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: service.CreatedDate,
                    secondDate: service.UpdatedDate,
                    secondDateName: nameof(Service.UpdatedDate)),

                    Parameter: nameof(Service.CreatedDate)));
        }

        private void ValidateServiceOnModify(Service service)
        {
            ValidateServiceNotNull(service);

            Validate(
				(Rule: IsInvalid(service.Id), Parameter: nameof(Service.Id)),
				(Rule: IsInvalid(service.TypeId), Parameter: nameof(Service.TypeId)),
				(Rule: IsInvalid(service.Name), Parameter: nameof(Service.Name)),
				(Rule: IsInvalid(service.SertificateNumber), Parameter: nameof(Service.SertificateNumber)),
				(Rule: IsInvalid(service.OwnerFIO), Parameter: nameof(Service.OwnerFIO)),
				(Rule: IsInvalid(service.Address), Parameter: nameof(Service.Address)),
				(Rule: IsInvalid(service.PhoneNumber), Parameter: nameof(Service.PhoneNumber)),
				(Rule: IsInvalid(service.CreatedDate), Parameter: nameof(Service.CreatedDate)),
				(Rule: IsInvalid(service.UpdatedDate), Parameter: nameof(Service.UpdatedDate)),

                (Rule: IsNotRecent(service.UpdatedDate), Parameter: nameof(Service.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: service.UpdatedDate,
                    secondDate: service.CreatedDate,
                    secondDateName: nameof(service.CreatedDate)),
                    Parameter: nameof(service.UpdatedDate)));
        }

        private static void ValidateAgainstStorageServiceOnModify(Service inputService, Service storageService)
        {
            ValidateStorageService(storageService, inputService.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputService.CreatedDate,
                    secondDate: storageService.CreatedDate,
                    secondDateName: nameof(Service.CreatedDate)),
                    Parameter: nameof(Service.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputService.UpdatedDate,
                        secondDate: storageService.UpdatedDate,
                        secondDateName: nameof(Service.UpdatedDate)),
                        Parameter: nameof(Service.UpdatedDate)));
        }

        private static void ValidateStorageService(Service maybeService, Guid serviceId)
        {
            if (maybeService is null)
            {
                throw new NotFoundServiceException(serviceId);
            }
        }

        private void ValidateServiceId(Guid serviceId) =>
             Validate((Rule: IsInvalid(serviceId), Parameter: nameof(Service.Id)));

        private void ValidateServiceNotNull(Service service)
        {
            if (service is null)
            {
                throw new NullServiceException();
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
            var invalidServiceException = new InvalidServiceException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidServiceException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidServiceException.ThrowIfContainsErrors();
        }
    }
}
