// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using CarManagement.Api.Brokers.DateTimes;
using CarManagement.Api.Brokers.Loggings;
using CarManagement.Api.Brokers.Storages;
using CarManagement.Api.Models.Cars;
using CarManagement.Api.Services.Foundations.Cars;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Cars
{
    public partial class CarServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICarService carService;

        public CarServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.carService = new CarService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<int> InvalidMinutes()
        {
            int minutesInFuture = GetRandomNumber();
            int minutesInPast = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                minutesInFuture,
                minutesInPast
            };
        }

        public static TheoryData<int> InvalidSeconds()
        {
            int secondsInPast = -1 * new IntRange(
                min: 60,
                max: short.MaxValue).GetValue();

            int secondsInFuture = new IntRange(
                min: 0,
                max: short.MaxValue).GetValue();

            return new TheoryData<int>
            {
                secondsInPast,
                secondsInFuture
            };
        }

        private IQueryable<Car> CreateRandomCars()
        {
            return CreateCarFiller(dates: GetRandomDatetimeOffset())
                .Create(count: GetRandomNumber()).AsQueryable();
        }

        private string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private DateTimeOffset GetRandomDatetimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static SqlException CreateSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static Car CreateRandomModifyCar(DateTimeOffset dates)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Car randomCar = CreateRandomCar(dates);

            randomCar.CreatedDate = randomCar.CreatedDate.AddDays(randomDaysInPast);

            return randomCar;
        }

        private static Car CreateRandomCar(DateTimeOffset dates) =>
           CreateCarFiller(dates).Create();

        private static Car CreateRandomCar() =>
            CreateCarFiller(GetRandomDateTime()).Create();

        private static Filler<Car> CreateCarFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Car>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
