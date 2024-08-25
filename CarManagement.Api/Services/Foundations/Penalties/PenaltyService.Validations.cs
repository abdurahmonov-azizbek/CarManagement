// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Penalties;
using CarManagement.Api.Models.Penalties.Exceptions;

namespace CarManagement.Api.Services.Foundations.Penalties
{
    public partial class PenaltyService
    {
        private void ValidatePenaltyOnAdd(Penalty penalty)
        {
            ValidatePenaltyNotNull(penalty);

            Validate(
				(Rule: IsInvalid(penalty.Id), Parameter: nameof(Penalty.Id)),
				(Rule: IsInvalid(penalty.TexPassportNumber), Parameter: nameof(Penalty.TexPassportNumber)),
				(Rule: IsInvalid(penalty.CarNumber), Parameter: nameof(Penalty.CarNumber)),
				(Rule: IsInvalid(penalty.EmployeeId), Parameter: nameof(Penalty.EmployeeId)),
				(Rule: IsInvalid(penalty.DriverId), Parameter: nameof(Penalty.DriverId)),
				(Rule: IsInvalid(penalty.CreatedDate), Parameter: nameof(Penalty.CreatedDate)),
				(Rule: IsInvalid(penalty.UpdatedDate), Parameter: nameof(Penalty.UpdatedDate)),

                (Rule: IsNotRecent(penalty.CreatedDate), Parameter: nameof(Penalty.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: penalty.CreatedDate,
                    secondDate: penalty.UpdatedDate,
                    secondDateName: nameof(Penalty.UpdatedDate)),

                    Parameter: nameof(Penalty.CreatedDate)));
        }

        private void ValidatePenaltyOnModify(Penalty penalty)
        {
            ValidatePenaltyNotNull(penalty);

            Validate(
				(Rule: IsInvalid(penalty.Id), Parameter: nameof(Penalty.Id)),
				(Rule: IsInvalid(penalty.TexPassportNumber), Parameter: nameof(Penalty.TexPassportNumber)),
				(Rule: IsInvalid(penalty.CarNumber), Parameter: nameof(Penalty.CarNumber)),
				(Rule: IsInvalid(penalty.EmployeeId), Parameter: nameof(Penalty.EmployeeId)),
				(Rule: IsInvalid(penalty.DriverId), Parameter: nameof(Penalty.DriverId)),
				(Rule: IsInvalid(penalty.CreatedDate), Parameter: nameof(Penalty.CreatedDate)),
				(Rule: IsInvalid(penalty.UpdatedDate), Parameter: nameof(Penalty.UpdatedDate)),

                (Rule: IsNotRecent(penalty.UpdatedDate), Parameter: nameof(Penalty.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: penalty.UpdatedDate,
                    secondDate: penalty.CreatedDate,
                    secondDateName: nameof(penalty.CreatedDate)),
                    Parameter: nameof(penalty.UpdatedDate)));
        }

        private static void ValidateAgainstStoragePenaltyOnModify(Penalty inputPenalty, Penalty storagePenalty)
        {
            ValidateStoragePenalty(storagePenalty, inputPenalty.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputPenalty.CreatedDate,
                    secondDate: storagePenalty.CreatedDate,
                    secondDateName: nameof(Penalty.CreatedDate)),
                    Parameter: nameof(Penalty.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputPenalty.UpdatedDate,
                        secondDate: storagePenalty.UpdatedDate,
                        secondDateName: nameof(Penalty.UpdatedDate)),
                        Parameter: nameof(Penalty.UpdatedDate)));
        }

        private static void ValidateStoragePenalty(Penalty maybePenalty, Guid penaltyId)
        {
            if (maybePenalty is null)
            {
                throw new NotFoundPenaltyException(penaltyId);
            }
        }

        private void ValidatePenaltyId(Guid penaltyId) =>
             Validate((Rule: IsInvalid(penaltyId), Parameter: nameof(Penalty.Id)));

        private void ValidatePenaltyNotNull(Penalty penalty)
        {
            if (penalty is null)
            {
                throw new NullPenaltyException();
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
            var invalidPenaltyException = new InvalidPenaltyException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidPenaltyException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidPenaltyException.ThrowIfContainsErrors();
        }
    }
}
