// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Offers;
using CarManagement.Api.Models.Offers.Exceptions;

namespace CarManagement.Api.Services.Foundations.Offers
{
    public partial class OfferService
    {
        private void ValidateOfferOnAdd(Offer offer)
        {
            ValidateOfferNotNull(offer);

            Validate(
				(Rule: IsInvalid(offer.Id), Parameter: nameof(Offer.Id)),
				(Rule: IsInvalid(offer.CarNumber), Parameter: nameof(Offer.CarNumber)),
				(Rule: IsInvalid(offer.TypeId), Parameter: nameof(Offer.TypeId)),
				(Rule: IsInvalid(offer.CreatedDate), Parameter: nameof(Offer.CreatedDate)),
				(Rule: IsInvalid(offer.UpdatedDate), Parameter: nameof(Offer.UpdatedDate)),

                (Rule: IsNotRecent(offer.CreatedDate), Parameter: nameof(Offer.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: offer.CreatedDate,
                    secondDate: offer.UpdatedDate,
                    secondDateName: nameof(Offer.UpdatedDate)),

                    Parameter: nameof(Offer.CreatedDate)));
        }

        private void ValidateOfferOnModify(Offer offer)
        {
            ValidateOfferNotNull(offer);

            Validate(
				(Rule: IsInvalid(offer.Id), Parameter: nameof(Offer.Id)),
				(Rule: IsInvalid(offer.CarNumber), Parameter: nameof(Offer.CarNumber)),
				(Rule: IsInvalid(offer.TypeId), Parameter: nameof(Offer.TypeId)),
				(Rule: IsInvalid(offer.CreatedDate), Parameter: nameof(Offer.CreatedDate)),
				(Rule: IsInvalid(offer.UpdatedDate), Parameter: nameof(Offer.UpdatedDate)),

                (Rule: IsNotRecent(offer.UpdatedDate), Parameter: nameof(Offer.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: offer.UpdatedDate,
                    secondDate: offer.CreatedDate,
                    secondDateName: nameof(offer.CreatedDate)),
                    Parameter: nameof(offer.UpdatedDate)));
        }

        private static void ValidateAgainstStorageOfferOnModify(Offer inputOffer, Offer storageOffer)
        {
            ValidateStorageOffer(storageOffer, inputOffer.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputOffer.CreatedDate,
                    secondDate: storageOffer.CreatedDate,
                    secondDateName: nameof(Offer.CreatedDate)),
                    Parameter: nameof(Offer.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputOffer.UpdatedDate,
                        secondDate: storageOffer.UpdatedDate,
                        secondDateName: nameof(Offer.UpdatedDate)),
                        Parameter: nameof(Offer.UpdatedDate)));
        }

        private static void ValidateStorageOffer(Offer maybeOffer, Guid offerId)
        {
            if (maybeOffer is null)
            {
                throw new NotFoundOfferException(offerId);
            }
        }

        private void ValidateOfferId(Guid offerId) =>
             Validate((Rule: IsInvalid(offerId), Parameter: nameof(Offer.Id)));

        private void ValidateOfferNotNull(Offer offer)
        {
            if (offer is null)
            {
                throw new NullOfferException();
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
            var invalidOfferException = new InvalidOfferException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidOfferException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidOfferException.ThrowIfContainsErrors();
        }
    }
}
